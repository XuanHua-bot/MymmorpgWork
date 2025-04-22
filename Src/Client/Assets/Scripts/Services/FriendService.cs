using System;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class FriendService : Singleton<FriendService>, IDisposable
    {
        public UnityAction OnFriendUpdate;

        public void Init()
        {
            
        }


        public FriendService()
        {  //订阅请求
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            //订阅相应们
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

      


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

       

        public void SendFriendAddRequest(int friendId, string friendName)
        {//我加别人  发送   
            Debug.Log("SendFriendAdd");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);

        }
        
        //根据OnFriendAddRequest  结果 相应，把消息发送给服务器
        public void SendFriendAddResponse(bool accept, FriendAddRequest request)
        {//别人 加我  接收     是否同意好友请求...
            Debug.Log("SendFriendAdd");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRes = new FriendAddResponse();
            message.Request.friendAddRes.Result = accept? Result.Success: Result.Failed;
            message.Request.friendAddRes.Errormsg = accept?"对方同意成为你的龟蜜":"对方拒绝成为你的龟蜜";
            message.Request.friendAddRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }




        /// <summary>
        /// 收到添加好友请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendAddRequest(object sender, FriendAddRequest request)
        {
            var confirm = MessageBox.Show(string.Format("[{0}] 想和你成为龟蜜", request.FromName), "好友请求", MessageBoxType.Confirm, "接受", "决绝");
            confirm.OnYes = () =>
            {
                this.SendFriendAddResponse(true, request);//request 是 发送好友请求
            };
            confirm.OnNo = () =>
            {
                this.SendFriendAddResponse(false, request);
            };
        }
        /// <summary>
        /// 收到添加好友相应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show(message.Request.ToName + "接受了您的请求", "龟蜜添加成功");
            else
                MessageBox.Show(message.Errormsg, "添加龟蜜失败");
        }
        
        
        
        private void OnFriendList(object sender, FriendListResponse message)
        {
           Debug.Log("OnFriendList");
           FriendManager.Instance.allFriends = message.Friends;
           if (this.OnFriendUpdate!=null)
           {
               this.OnFriendUpdate();
           }
        }
        
        
        public void SendFriendRemoveRequest (int id, int friendId)
        {
            Debug.Log("SendFrienRemoveReuqest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;
            message.Request.friendRemove.friendId = friendId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnFriendRemove(object sender, FriendRemoveResponse message)
        {
            if (message.Result == Result.Success)
            {
                MessageBox.Show("删除成功", "删除龟蜜");
            }
            else
            {
                MessageBox.Show("删除失败", "删除龟蜜",MessageBoxType.Error);
            }
        }
        
        
        
    }
}