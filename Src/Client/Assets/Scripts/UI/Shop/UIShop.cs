using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow {

    public Text title;
    public Text money;

    
    public GameObject shopItem;
    ShopDefine shop;

    public Transform[] itemRoot;//页数

   
    private void Start()
    {
        
       
        StartCoroutine(InitItems());
    }

    IEnumerator InitItems()
    {

        
        int count = 0;
        int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)//道具得到状态 0为不可销售
            {
                GameObject go = Instantiate(shopItem, itemRoot[page]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
                count++;
                if (count>=12)
                {
                    count = 0;
                    page++;
                    itemRoot[page].gameObject.SetActive(true);
                }
            }
            yield return null;
        }

        

        
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private UIShopItem selectedItem;//被选中项
    public void SelectShopItem(UIShopItem item)
    {
        if (selectedItem !=null)
        {
            selectedItem.Selected = true;
        }
        selectedItem = item;
    }

    public void OnclickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "道具提示");
            return;
        }
        //发送的是 商店中 的 道具序列   会在服务器中进行校验  
        //                              发送 商店 id  与  被选中物体的 id
        if(ShopManager.Instance.BuyItem(this.shop.ID,this.selectedItem.ShopItemID))
        {
            MessageBox.Show("选择了道具："+this.selectedItem, "道具提示");
        }
    }
}
