﻿using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum ItemFunction
    {
        RecoverHP,
        RecoverMP,
        AddBuff,
        AddExp,
        AddMoney,
        AddItem,
        AddSkillPoint,
    }





    public class ItemDefine
    {
        public int ID {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public ItemType Type {get;set;}
        public string Category {get;set;}
        public int Level { get; set; }//装备等级
        public CharacterClass LimitClass { get; set; }//装备职业限制
        public bool CanUse {get;set;}
        public float UseCD {get;set;}
        public int Price {get;set;}
        public int SellPrice {get;set;}

        public int StackLimit { get; set; }//堆叠限制
        public string Icon { get; set; }//图标

        public ItemFunction  Function {get;set;}//道具功能
        public int Param { get; set; }
        public List<int> Params { get; set; }

    }
}
