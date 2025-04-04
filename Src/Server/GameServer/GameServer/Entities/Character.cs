using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    /// <summary>
    /// Character
    /// 玩家角色类
    /// </summary>
    class Character : CharacterBase
    {
       
        public TCharacter Data;

        public ItemManager ItemManager;

        //public StatusManager StatusManager;

        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.Name = cha.Name;
            this.Info.Level = 1;//cha.Level;
            this.Info.Tid = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.Gold = cha.Gold;
            this.Info.mapId = cha.MapID;
            this.Info.Entity = this.EntityData;



            this.ItemManager = new ItemManager(this);//构建道具管理器
            this.ItemManager.GetItemInfos(this.Info.Items);////从内存数据转换为网络数据

            //从数据库拉去角色背包
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.Bag.UnLocked; // 格子解锁状态
            this.Info.Bag.Items = this.Data.Bag.Items; //道具列表

        }
    }
}
