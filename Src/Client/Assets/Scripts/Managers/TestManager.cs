using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class TestManager : Singleton<TestManager>
    {

        public void Init()
        {
            NPCManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeShop, OnNpcInvokeShop);
            NPCManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeInsrance, OnNpcInvokeInsrance);
        }
        private bool OnNpcInvokeShop(NpcDefine npc)
        {
            Debug.LogFormat("TestManager,OnNpcInvokeShop NPC:[{0}{1}] Type :{2} Func :{3}", npc.ID, npc.Name, npc.Type, npc.Function);
            UITest uITest =  UIManager.Instance.Show<UITest>();
            uITest.SetTitle(npc.Name+"的测试商店 ");
            return true;
        }
        private bool OnNpcInvokeInsrance(NpcDefine npc)
        {
            Debug.LogFormat("TestManager,OnNpcInvokeShop NPC:[{0}{1}] Type :{2} Func :{3}", npc.ID, npc.Name, npc.Type, npc.Function);
            MessageBox.Show("点击了 NPC：" + npc.Name + "NPC对话");
            return true;
        }



    }
}
