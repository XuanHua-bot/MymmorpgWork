using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using SkillBridge.Message;

namespace Models
{
    public class Item//同时维护道具与装备
    {
        public int Id;
        public int Count;
        
        public ItemDefine Define;//道具信息
        public EquipDefine EquipInfo;//装备信息
        
        
        //构造函数链式调用
        //当用 NItemInfo 对象创建 Item 时，其实只需要取出它的 Id 和 Count 这两个值，然后调用另一个专门接收 Id 和 Count 的构造函数来干活"
        public Item(NItemInfo item) : this(item.Id, item.Count)   
        {
        }
           
        
        public Item(int id,int count)
        {
            this.Id = id;
            this.Count = count;
            //this.Define = DataManager.Instance.Items[this.Id];
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id,out this.EquipInfo);
        }
        public override string ToString()
        {
            return string.Format("Id:{0},Count:{1}", this.Id, this.Count);
        }

    }
}
