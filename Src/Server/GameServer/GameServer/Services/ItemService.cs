using System;
using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace GameServer.Services
{
   public class ItemService:Singleton<ItemService>
    {
        public ItemService()
        {

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);
        }

        

        public void Init()
        {
            
        }
        
        private void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemBuy:Character:{0},Shop：{1},ShopItem:{2}",sender.Session.Character,request.shopId,request.shopItemId);
            var result = ShopManager.Instance.BuyItem(sender, request.shopId, request.shopItemId);
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.SendResponse();
            //todo
            //物品购买需完成


        }

        
        
        private void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnItemEquip: :character:{0}:Slot:{1} Item:{2} Equip:{3}",character.Id, request.Slot, request.itemId, request.Equip);
            var result = EquipManager.Instance.EquipItem(sender, request.Slot, request.itemId, request.Equip);
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }
    }
}