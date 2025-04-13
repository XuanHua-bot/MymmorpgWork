using System;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    public class ItemService : Singleton<ItemService>,IDisposable
    {
        
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }

       

        public void SendBuyItem(int shopId, int shopItemId)
        {
            Debug.Log("SendBuyItem");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(message);
        }
        
        
        
        
        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            var msg =MessageBox.Show(
                "购买结果：" + message.Result + "\n" + message.Errormsg, // 消息内容
                "购买完成" // 标题
            );
            msg.OnYes = () =>
            {
                UIShop uiShop = GameObject.Find("UIShop(Clone)").GetComponent<UIShop>();
                if (uiShop != null)
                {
                    uiShop.InitItems();
                }
            };

        }


        private Item pendingEquip = null;
        private bool isEquip;

        public bool sendEquipItem(Item equip,bool isEquip)
        {
            if (pendingEquip!=null)
            {
                return false;
            }
            Debug.Log("SendEquipItem");

            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;
            message.Request.itemEquip.itemId = equip.Id;
            message.Request.itemEquip.Equip = isEquip;
            NetClient.Instance.SendMessage(message);
            return true;
        }
        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (pendingEquip !=null)
                {
                    if (this.isEquip)
                    {
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    }
                    else
                    {
                       EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);
                    }

                    pendingEquip = null;
                }
            }
        }
    }
}