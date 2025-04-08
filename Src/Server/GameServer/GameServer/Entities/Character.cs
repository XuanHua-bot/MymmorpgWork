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

        public StatusManager StatusManager;

        //角色类的构造函数，进行一系列的初始化
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


            
            //从数据库拉去角色背包 进行背包初始化
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.Bag.UnLocked; // 格子解锁状态
            this.Info.Bag.Items = this.Data.Bag.Items; //道具列表

            this.Info.Equips = this.Data.Equips;//装备

            //角色 增删改状态 初始化
            this.StatusManager = new StatusManager(this);

        }

        public long Gold 
        {
            get { return this.Data.Gold; }
            set
            {
                if (this.Data.Gold == value)
                {
                    return;
                }
                this.StatusManager.AddGoldChange((int)(value-this.Data.Gold));
                this.Data.Gold = value;
            }
        }
    }
}
