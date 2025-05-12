using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Team
    {
        public int Id;
        public Character Leader;
        public List<Character> Members = new List<Character>();//成员 

        public double timestamp;

        public Team(Character leader)//new Team时先设置队长
        {
            this.AddMember(leader);
        }


        public void AddMember(Character member)
        {
            if (this.Members.Count == 0)//判断有无成员
            {
                this.Leader = member;//设置为队长
            }
            this.Members.Add(member);//添加到成员
            member.Team = this;//当前队伍指定给该成员
            timestamp = TimeUtil.timestamp;//队伍信息变更的时间
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("Leave Team : {0}:{1}",member.Id,member.Info.Name);
            this.Members.Remove(member);//从队伍中移除成员
            if (member==this.Leader)
            {
                if (this.Members.Count>0)//如果还有人
                {
                    this.Leader = this.Members[0];//队伍第一个人为队长
                }
                else
                    this.Leader = null;
            }
            member.Team = null;
            timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.teamInfo ==null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Team = new NTeamInfo();
                message.teamInfo.Team.Id = this.Id;
                message.teamInfo.Team.Leader = this.Leader.Id;
                foreach (var member in this.Members)
                {
                    message.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
           
            

        }
    }
}