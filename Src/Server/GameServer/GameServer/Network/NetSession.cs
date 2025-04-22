using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }// 相应后处理器

        public void Disconnected()
        {
            this.PostResponser = null;//断开时清空
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }

        private NetMessage response;//根消息

        public NetMessageResponse Response//response 属性  在session随时取得response
        {
            get
            {
                if (response==null)
                {
                    response = new NetMessage();
      
               }

                if (response.Response==null)
                {
                    response.Response = new NetMessageResponse();
                }

                return response.Response;
            }
        }

        //todo
        //代码需补全
        public byte[] GetResponse()
        {
            if (response !=null)
            {
                if (PostResponser!=null)
                {
                    this.PostResponser.PostProcess(Response);
                }
               /* if (this.Character!=null && this.Character.StatusManager.HasStatus)//如果角色管理器上 有状态
                {
                    this.Character.StatusManager.ApplyResponse(Response);
                }
*/
                byte[] data = PackageHandler.PackMessage(response);// response 发送给客户端后 立刻清空
                response = null;
                return data;
            }

            return null;
        }
    }
}
