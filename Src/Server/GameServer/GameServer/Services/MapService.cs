using Common;
using Common.Data;
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
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
         //MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapCharacterEnterRequest>(this.OnMapCharacterEnter);
         MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
         MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleTeleport);

        }

        

        public void Init()
        {
            MapManager.Instance.Init();
        }
        

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)//接收与广播同步请求
        {
            Character character = sender.Session.Character;//通过sender.Session.Character获取当前玩家角色，确保更新正确实体。
            Log.InfoFormat("OnMapEntitySync: characterID: {0}:{1} Entity.Id:{2} Evt:{3} Entity:{4}",character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());


            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        internal void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entity);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }

        void OnMapTeleTeleport(NetConnection<NetSession> sender ,MapTeleportRequest request)
        {
            Character character = sender.Session.Character;//请求传送角色
            Log.InfoFormat("OnMapTeleport:characterID:{0}:{1} TeleporterID:{2}", character.Id, character.Data, request.teleporterId);
            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId)) //检查传送点是否存在
            {
                Log.WarningFormat("Source TeleportID [{0}]not existed", request.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];//存在则读取传送点
            if (source.LinkTo == 0 || DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))//检查目标传送点是否存在
            {
                Log.WarningFormat("Source TeleporterID [{0}] LinkTo[{1}] not existed", request.teleporterId, source.LinkTo);
            }
            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];//读取目标传送点

            MapManager.Instance[source.MapID].CharacterLeave(character);//角色离开当前地图

            MapManager.Instance[target.MapID].CharacterEnter(sender, character);//角色进入传送点地图

            //根据表格设置玩家 位置 方向
            character.Position = target.Position;
            character.Direction = target.Direction;
            
        }
    }
}
