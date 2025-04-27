using Common;
using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    //todo  FriendManager 待补全

    /// <summary>
    /// Character
    /// 玩家角色类
    /// </summary>
    class Character : CharacterBase,IPostResponser
    {
       
        public TCharacter Data;

        public ItemManager ItemManager;

        public QuestManager QuestManager;

        public StatusManager StatusManager;

        public FriendManager FriendManager;

        public Team Team;//储存队伍
        public double TeamUpdateTS;//TS 为时间戳

        //角色类的构造函数，进行一系列的初始化
        public Character(CharacterType type,TCharacter cha):
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Id = cha.ID;
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId;
            this.Info.Name = cha.Name;
            this.Info.Level = 10;//cha.Level;
            this.Info.ConfigId = cha.TID;
            //his.Info.Tid = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.Gold = cha.Gold;
            this.Info.mapId = cha.MapID;
            this.Info.Entity = this.EntityData;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];



            this.ItemManager = new ItemManager(this);//构建道具管理器
            this.ItemManager.GetItemInfos(this.Info.Items);////从内存数据转换为网络数据


            
            //从数据库拉去角色背包 进行背包初始化
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.Bag.UnLocked; // 格子解锁状态
            this.Info.Bag.Items = this.Data.Bag.Items; //道具列表

            this.Info.Equips = this.Data.Equips;//装备

            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);
            
            
            //角色 增删改状态 初始化
            this.StatusManager = new StatusManager(this);

            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);
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


        public void PostProcess(NetMessageResponse message)//实现的后处理的接口  
        {
            Log.InfoFormat("PostProcess > Character ： characterID:{0}:{1}",this.Id,this.Info.Name);
            this.FriendManager.PostProcess(message);
            
           
            if (this.Team!= null)//判断当前有无队伍
            {
                
                Log.InfoFormat("PostProcess > Team: characterID:{0}:{1} {2}",this.Id,this.Info.Name,TeamUpdateTS,this.Team.timestamp);
                //  自己队伍信息的时间戳（默认为0） < 队伍信息
                if (TeamUpdateTS<this.Team.timestamp)
                { 
                    //如果 比队伍的时间戳小 则 更新
                    //每个玩家的 =   队伍的时间戳
                    TeamUpdateTS = Team.timestamp;
                    this.Team.PostProcess(message);
                }
            }

            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }
           
        }


        /// <summary>
        /// 角色离开时调用
        /// </summary>
        public void Clear()
        {
            this.FriendManager.offlineNotify();//离线通知
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level,
            };
        }
    }
}
