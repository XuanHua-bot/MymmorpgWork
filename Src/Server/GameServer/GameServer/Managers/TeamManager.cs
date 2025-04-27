using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using Common;
using GameServer.Entities;
using GameServer.Models;

namespace GameServer.Managers
{ 
    class TeamManager: Singleton<TeamManager>
    {
        public List<Team> Teams = new List<Team>();//队伍创建了 不会销毁   可以复用

        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();
        
        public void Init(){}

        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            this.CharacterTeams.TryGetValue(characterId, out team);
            return team;
        }

        public void AddTeamMember(Character leader, Character member)
        {
            if (leader.Team == null)//队长有无队伍
            {
                leader.Team = CreateTeam(leader);//创建队伍 并设置队长
            }

            leader.Team.AddMember(member);//队长有队伍  添加队员
        }

        private Team CreateTeam(Character leader)
        {
            Team team = null;
            for (int i = 0; i < this.Teams.Count; i++)// 遍历 List<Team> Teams
            {
                team = this.Teams[i];
                if (team.Members.Count == 0)//查询当前队伍成员是否为空
                {
                    team.AddMember(leader);//把自己添加进去（使用空的队伍 来优化内存  省去分配内存创建新队伍）
                    return team;
                }

               
            }
            team = new Team(leader);
            this.Teams.Add(team);
            team.Id = this.Teams.Count;//队伍只增不减 所以 使用队伍数量作为队伍id
            return team;
        }
    }
}