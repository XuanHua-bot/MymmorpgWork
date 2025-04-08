using Common;
using Common.Data;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ShopManager:Singleton<ShopManager>//单例 服务于多人  所以传入sender
    {
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int shopItemId)
        {
            if (!DataManager.Instance.Shops.ContainsKey(shopId))//判断是否存在
            {
                return Result.Failed;
            }

            ShopItemDefine shopItem;
            if (DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId,out shopItem))//判断是否存在
            {
                Log.InfoFormat("BuyItem :Character:{0} ,Item:{1} , Count{1}, Price{3}",sender.Session.Character.Id,shopItem.ItemID,shopItem.Count,shopItem.Price);
                if (sender.Session.Character.Gold>=shopItem.Price)
                {
                    //金币 足够 则执行 物品添加和金币扣除逻辑
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }
            }
            return Result.Failed;

        }
    }
}