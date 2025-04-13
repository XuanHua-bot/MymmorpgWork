using Models;
using Common.Data;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;
using Services;

namespace Managers
{
    class ItemManager:Singleton<ItemManager>//负责管理玩家物品的本地数据
    {
        
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        internal void Init(List<NItemInfo> items)
        {
            
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);
                //输出现有道具
                Debug.LogFormat("ItemManager:Init[{0}]",item);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item,OnItemNotify);//注册通知
        }

        

        public ItemDefine GetItem(int itemId)
        {
            return null;
        }
        
        private bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }

            if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }

            return true;
        }

        private void AddItem(int itemId, int count)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId,out item))//如果玩家已经有了
            {
                item.Count += count;
            }
            else
            {
                item = new Item(itemId, count);//调用item类的构造函数
                this.Items.Add(itemId,item);
            }

            //道具更新了 ，背包也要更新
            BagManager.Instance.AddItem(itemId, count);
        }

        void RemoveItem(int itemId, int count)
        {
            if (!this.Items.ContainsKey(itemId))
            {
                return;
            }

            Item item = this.Items[itemId];
            if (item.Count<count)
            {
                return;
            }

            item.Count -= count;
            //道具更新了 ，背包也要更新
            //BagManager.Instance.RemoveItem(itemId, count);
            //todo 
            //此处应补全
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }


    }
}
