using System.Collections.Generic;
using SkillBridge.Message;

namespace Managers
{
    public class FriendManager : Singleton<FriendManager>
    {
       //好友列表
       public List<NFriendInfo> allFriends;

       public void Init(List<NFriendInfo> friends)
       {
           this.allFriends = friends;
       }
    }
}