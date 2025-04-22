using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class FriendService: Singleton<FriendService>
    {
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }

      


        public void Init()
        {

        }


        /// <summary>
        /// 收到添加好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRequest::FromId {0} FromName {1} ToID: {2} ToName: {3}", request.FromId, request.FromName, request.ToId, request.ToName);

            if (request.ToId == 0)
            {
                //如果没有传入id  则使用名称进行查找

                foreach (var cha in CharacterManager.Instance.Characters)//此处为在线的玩家
                {
                    if (cha.Value.Data.Name== request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }

            NetConnection<NetSession> friend = null;//查询 要加的好友的session 
            if (request.ToId >0)
            {
                //此处character 为 当前 session 玩家   查询 要添加的好友是否已经添加过了
                if (character.FriendManager.GetFriendInfo(request.ToId)!=null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是龟蜜了";
                    sender.SendResponse();
                    return;
                }
                //如果不是好友 就从sessionManager 根据角色的 （DBid 只有角色有）id 获取session
                friend = SessionManager.Instance.GetSession(request.ToId);
            }
            if (friend == null)//session 没有 可能玩家掉线 或 不存在
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "该玩家不存在或不在线噶";
                sender.SendResponse();
                return;
            }

            // 满足条件 则发送 好友请求
            Log.InfoFormat("ForwardRequest :: FromId : {0} FromName:{1} ToID:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            friend.Session.Response.friendAddReq = request;// 转发给 好友session
            friend.SendResponse();
        }

       


        /// <summary>
        /// 收到添加好友相应
        /// </summary>
        /// <param name="netConnection"></param>
        /// <param name="response"></param>
        void OnFriendAddResponse(NetConnection<NetSession> sender,FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse ::character:{0} Result:{1} FromId:{2} ToID{3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.friendAddRes = response;
            if (response.Result==Result.Success)
            {
                //接受了好友请求 
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "请求者已下线";
                }
                else
                {
                    //互相加好友
                    character.FriendManager.AddFriend(requester.Session.Character);//a+b
                    requester.Session.Character.FriendManager.AddFriend(character);//b+a
                    DBService.Instance.Save();//保存数据库
                    requester.Session.Response.friendAddRes = response;
                    sender.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加龟蜜成功";
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }
        
        
        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove: :character:{0} FriendReletionID:{1}", character.Id, request.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;
            //删除自己的好友
            if (character.FriendManager.RemoveFriendByID(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                //删除别人好友中的自己
                var friend = SessionManager.Instance.GetSession(request.friendId);
                if (friend!=null)
                {//好友在线
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id);//在线的话 需要删除内存 不是很理解
                }
                else
                { //好友不在线
                    this.RemoveFriend(request.friendId, character.Id);//不在线直接删除数据库
                }
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;

            DBService.Instance.Save();
            
            sender.SendResponse();
        }
        

        void RemoveFriend(int charId, int friendId)
        {
            var removeItem = DBService.Instance.Entities.TCharacterFriends.FirstOrDefault(v => v.CharacterID == charId && v.FriendID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
        }

    }
}
