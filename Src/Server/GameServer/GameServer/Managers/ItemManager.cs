﻿using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers//主要负责管理玩家角色的物品，包括物品的初始化、使用、判断是否拥有、获取、添加和移除等操作。
{
    class ItemManager
    {

        Character Owner;

        //维护所有角色身上的道具 //注意 非单例
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();//维护一个道具字典 省去多次查询数据库

        public ItemManager(Character owner)//构造函数  物体运行时 自动执行 itemmanager  进行 道具的List初始化
        {
            this.Owner = owner;

            //                    玩家 数据的 物品
            foreach (var item in owner.Data.Items)
            {
                //添加到字典 
                this.Items.Add(item.ItemID, new Item(item));//为每个物品创建一个 Item 对象，并将其添加到 Items 字典中，键为物品的 ID。
            }
        }

        public bool UseItem(int itemId,int count = 1)//使用物品ID，使用物品个数 默认一个
        {
            Log.InfoFormat("[{0}]UserItem[{1}:{2}]", this.Owner.Data.ID, itemId, count);
            Item item = null;
            if (this.Items.TryGetValue(itemId,out  item))//尝试从 Items 字典中获取指定 ID 的物品。
            {
                if (item.Count<count)//检查 字典内的 物品个数 是否足够
                {
                    return false;
                }

                //TODO
                //道具使用逻辑 待完成：：：：


                item.Remove(count);

                return true;
            }
            return false;
        }

        public bool HasItem(int itemId)//判断道具是否存在
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId,out item))
            {
                return item.Count > 0;
            }
            return false;
        }
        public Item GetItem(int itemId)//获取道具
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]GetItem[{1}]:[{2}]", this.Owner.Data.ID, itemId, item);
            return item;//返回获取到的物品对象，如果不存在则返回 null。
        }

        public bool AddItem(int itemId,int count)//增加道具
        {
            Item item = null;

            if (this.Items.TryGetValue(itemId,out item))//尝试从 Items 字典中获取指定 ID 的物品。
            {
                item.Add(count);  //调用 item.Add(count) 方法增加物品数量。
            }
            else//如果该道具不存在 则在数据库插入道具
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.TCharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                //插入字典
                item = new Item(dbItem);
                this.Items.Add(itemId, item);
            }
            this.Owner.StatusManager.AddItemChange(itemId,count,StatusAction.Add);
            Log.InfoFormat("[{0}]AddItem[{1}] addCount[{2}]", this.Owner.Id, item, count);
            //DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int ItemId,int count)//增删 部分 都需要数据库参与
        {
            if (!this.Items.ContainsKey(ItemId))//判断物品是否存在 不存在 不执行移除
            {
                return false;
            }
            Item item = this.Items[ItemId];
            if (item.Count<count)
            {
                return false;
            }
            item.Remove(count);
            Owner.StatusManager.AddItemChange(ItemId,count,StatusAction.Delete);
            Log.InfoFormat("[{0} RemoveItem[{1}]] removeCount:{2}", this.Owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true; 
        }

        public void GetItemInfos(List<NItemInfo> list)//从内存数据转换为网络数据
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
