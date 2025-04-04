using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Services;

namespace Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            //                                   注册的npc的事件         注册的方法是openshop
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);//Param NpcDefine中定义的商店1，2
            return true;
        }

        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId, out shop))//查询shangdianid是否存在
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();//把读取到的商店定义设置给shop
                if (uiShop != null)
                {
                    uiShop.SetShop(shop);
                }
            }
        }

        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }
    }
}
