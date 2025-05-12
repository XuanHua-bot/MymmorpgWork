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
    class GuildService : Singleton<GuildService>
    {

        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }

       

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnGuildCreate(NetConnection<NetSession>sender,GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate:GuildName{0} character:{1}{2}", request.GuildName, character.Id, character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild!=null)//检查是否 已有工会
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate .Errormsg = " 已有工会";
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(request.GuildName))//检查工会名称 是否存在
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "工会名称已存在";
                sender.SendResponse();
                return;
            }
            //创建公会
            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();

        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList: character:[{0}][{1}]", character.Id, character.Name);


            sender.Session.Response.guildList = new GuildListResponse ();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }


        private void OnGuildJoinRequest(NetConnection<NetSession> sender,GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRequest ：：GuildId:{0} characterId :[{1}][{2}]", request.Apply.GuildId, request.Apply.characterId, request.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)//验证 工会id是否存在
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Success;
                sender.Session.Response.guildJoinRes.Errormsg = "工会不存在";
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;

            if (guild.JoinApply(request.Apply))//把 工会申请 加入 申请列表
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if (leader !=null)
                {//给会长发送申请加入请求
                    leader.Session.Response.guildJoinReq = request;//请求转发给会长
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复申请";
                sender.SendResponse();


            }
        }


        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinResponse::GuildId:{0} characterId:[{1}][{2}]",response.Apply.GuildId,response.Apply.characterId,response.Apply.characterId);


            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            if (response.Result == Result.Success)
            {//接受了工会请求
                guild.JoinAppove(response.Apply);//工会审批
            }

            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if (requester!=null)
            {
                requester.Session.Character.Guild = guild;


                requester.Session.Response.guildJoinRes = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入工会成功";
                requester.SendResponse();
            }
        }


        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave::character:{0}", character.Id);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            character.Guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;
            DBService.Instance.Save();
            sender.SendResponse();

        }



        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;//获取当前角色
            Log.InfoFormat("OnGuildLeave ::character:{0}", character.Id);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if (character.Guild==null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.Guild.Errormsg = "你哪来的工会";
                sender.SendResponse();
                return;
            }
            //加入成功了
            character.Guild.ExecuteAdmin(message.Command,message.Target,character.Id);

            var target = SessionManager.Instance.GetSession(message.Target);
            if (target !=null)//如果他在线 则给他发通知
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.Session.Response.guildAdmin.Command = message;
                target.SendResponse();
            }
            //返回调用目标
            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Command = message;
            sender.SendResponse();
        }



    }
}
