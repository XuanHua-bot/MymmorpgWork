using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using SkillBridge.Message;

namespace Models
{
    class Item
    {
        public int Id;
        public int Count;
        public ItemDefine Define;

        //使用的网络协议中的iteminfo   //客户端来源于网络 服务端来源于DB
        public Item(NItemInfo item)
        {
            this.Id = item.Id;
            this.Count = item.Count;
            this.Define = DataManager.Instance.Items[item.Id];
        }
        public override string ToString()
        {
            return string.Format("Id:{0},Count:{1}", this.Id, this.Count);
        }

    }
}
