using System.Collections.Generic;
using Common;
using Network;

namespace GameServer.Managers
{ 
    class SessionManager: Singleton<SessionManager>
    {
        //             角色id  , 每个玩家对应的session
        public Dictionary<int, NetConnection<NetSession>> Sessions = new Dictionary<int, NetConnection<NetSession>>();

        public void AddSession(int characterId, NetConnection<NetSession> session)
        {
            this.Sessions[characterId] = session;
        }

        public void RemoveSession(int characterId)
        {
            this.Sessions.Remove(characterId);
        }

        public NetConnection<NetSession> GetSession(int characterId)
        {
            NetConnection<NetSession> session = null;
            this.Sessions.TryGetValue(characterId, out session);
            return session;
        }
    }
}