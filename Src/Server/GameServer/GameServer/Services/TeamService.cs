using System.Collections.Generic;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace GameServer.Services
{
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<teamInviteRequest>(
                this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(
                this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }


        public void Init()
        {
            TeamManager.Instance.Init();
        }


        /// <summary>
        ///  收到组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTeamInviteRequest(NetConnection<NetSession> sender, teamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteReuqest：：FromID:{0} FromeName:{1} ToID :{2}  ToName:{3} ", request.FromId,
                request.FromName, request.TeamId, request.ToName);

            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            Log.InfoFormat("--------------------从 netsession中获取的角色 id：{0} name:{1}", target.Session.Character.Id,
                target.Session.Character.Info.Name);
            if (target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "好友不在线！";
                sender.SendResponse();
                return;
            }

            if (target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已经有队伍";
                sender.SendResponse();
                return;
            }

            //转发请求
            Log.InfoFormat("-----ForwardTeamInviteRequest: : FromId:{0} FromName:{1} ToID:{2} ToName:{3}",
                request.FromId, request.FromName, request.ToId, request.ToName); //此处toi from i 相同
            Log.InfoFormat("-----ForwardTeamInviteRequest: : FromId:{0} FromName:{1} ToID:{2} ToName:{3}",
                request.FromId, request.FromName, target.Session.Character.Id, target.Session.Character.Info.Name);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        /// <summary>
        /// 收到队伍相应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteResponse :: character:{0} Result:{1} FromeId:{2} ToId{3}", character.Id,
                response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.teamInviteRes = response;

            if (response.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "请求者已下线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse();

                    // 向所有团队成员发送 TeamInfoResponse
                    Models.Team team = character.Team;
                    foreach (var member in team.Members)
                    {
                        NetConnection<NetSession> memberSession = SessionManager.Instance.GetSession(member.Id);
                        if (memberSession != null)
                        {
                            memberSession.Session.Response.teamInfo = new TeamInfoResponse();
                            memberSession.Session.Response.teamInfo.Result = Result.Success;
                            memberSession.Session.Response.teamInfo.Team = new NTeamInfo();
                            memberSession.Session.Response.teamInfo.Team.Id = team.Id;
                            memberSession.Session.Response.teamInfo.Team.Leader = team.Leader.Id;
                            foreach (var memberChar in team.Members)
                            {
                                memberSession.Session.Response.teamInfo.Team.Members.Add(memberChar.GetBasicInfo());
                            }

                            memberSession.SendResponse();
                        }
                    }
                }
            }

            sender.SendResponse();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;

            Log.InfoFormat("OnTeamLeave : :character:{0} TeamID:{1} : {2}", character.Id, request.TeamId,
                request.characterId);
            Models.Team team = character.Team;

            if (team != null) team.Leave(character);

            foreach (var member in team.Members)
            {
                NetConnection<NetSession> memberSession = SessionManager.Instance.GetSession(member.Id);

                memberSession.Session.Response.teamInfo = new TeamInfoResponse();
                memberSession.Session.Response.teamInfo.Result = Result.Success;
                memberSession.Session.Response.teamInfo.Team = new NTeamInfo();
                memberSession.Session.Response.teamInfo.Team.Id = team.Id;
                memberSession.Session.Response.teamInfo.Team.Leader = team.Leader.Id;
                foreach (var cha in team.Members)
                {
                    memberSession.Session.Response.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }

                memberSession.SendResponse();
            }

            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.Result = Result.Success;
            sender.Session.Response.teamLeave.Errormsg = character.Id == request.characterId ? "退出成功！" : "提出成功！";
            sender.Session.Response.teamLeave.characterId = request.characterId;

            sender.SendResponse();
        }
    }
}