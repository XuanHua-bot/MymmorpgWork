using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class UIBag:UIWindow
    {
        public Text money;

        public Transform[] pages;//页数

        public GameObject bagItem;//显示的图标和文本

        List<Image> slots; //槽  格子


        private void Start()
        {
            if (slots ==null)
            {
                slots = new List<Image>();
                for (int page = 0; page < this.pages.Length; page++)
                {
                    slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));

                }
               
            }
            StartCoroutine(InitBags());
        }

        IEnumerator InitBags()
        {
            for(int i = 0; i < BagManager.Instance.Items.Length; i++)
            {
                var item = BagManager.Instance.Items[i];
                if (item.ItemId>0&&item.Count>0)
                {
                   
                    //                                  设置 父节点
                    GameObject go = Instantiate(bagItem, slots[i].transform);
                    var ui = go.GetComponent<UIIconItem>();
                    //从道具管理器中获取道具配置
                    var def = ItemManager.Instance.Items[item.ItemId].Define;
                    //设置图标与 数量
                    ui.SetMainIcon(def.Icon, item.Count.ToString());
                }
            }
            //当前背包中已经解锁的格子    一共有多少槽
            for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
            {
                //其余的格子设置为灰色
                slots[i].color = Color.gray;
            }
            yield return null;
        }
        public void SetTile(string title)
        {
            this.money.text = User.Instance.CurrentCharacter.Id.ToString();
        }

        public void Clear()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].transform.childCount>0)
                {
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }
            }
        }
        
        public void OnRest()
        {
            BagManager.Instance.Reset();
            this.Clear();
            StartCoroutine(InitBags());
        }
    }

    
}
