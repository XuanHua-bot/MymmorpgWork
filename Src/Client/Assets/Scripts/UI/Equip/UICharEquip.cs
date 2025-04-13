using System;
using System.Collections.Generic;
using Managers;
using Models;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UICharEquip: UIWindow
    {
        public Text title;
        public Text money;

        public GameObject itemPrefab;
        public GameObject itemEqiupedPrefab;

        public Transform itemListRoot;

        public List<Transform> slots;


        private void Start()
        {
            RefreshUI();
            EquipManager.Instance.OnEquipChanged += RefreshUI;//随时执行  更新ui方法
        }

       

        private void OnDestroy()
        {
            EquipManager.Instance.OnEquipChanged -= RefreshUI;
        }
        
        
        private void RefreshUI()
        {
            ClearAllEquipList();//清空左侧装备列表
            InitAllEquipItems();//初始化 左侧装备列表
            ClearEquipedList();//清空装备栏
            initEquipedItems();//初始化装备栏
            this.money.text = User.Instance.CurrentCharacter.Gold.ToString();//刷新金钱
        }

        


        /// <summary>
        /// 初始化所有装备列表
        /// </summary>
        /// <returns></returns>
        private void InitAllEquipItems()
        {
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Define.Type == ItemType.Equip&& kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class)//只显示 装备 类型的物品
                {
                    if (EquipManager.Instance.Contains(kv.Key))
                    {
                        continue;
                    }

                    //如果 装备列表没有该物品，则实例化prefab
                    GameObject go = Instantiate(itemPrefab, itemListRoot);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(kv.Key,kv.Value,this,false);
                }
            }
        }
        private void ClearAllEquipList()
        {
            foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
            {
                Destroy(item.gameObject);
            }
        }
        
        private void ClearEquipedList()
        {
            foreach (var item in slots)
            {
                if (item.childCount>0)
                {
                    Destroy(item.GetChild(0).gameObject);
                }
            }
        }
        
        private void initEquipedItems()
        {
            for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
            {
                var item = EquipManager.Instance.Equips[i];
                {
                    if (item !=null)
                    {
                        GameObject go = Instantiate(itemEqiupedPrefab, slots[i]);
                        UIEquipItem ui = go.GetComponent<UIEquipItem>();
                        ui.SetEquipItem(i,item,this,true);
                    }
                }
                
                
            }
            
        }

        public void DoEquip(Item item)//穿装备
        {
            EquipManager.Instance.EquipItem(item);
        }

        public void UnEquip(Item item)//脱装备
        {
            EquipManager.Instance.UnEquipItem(item);
        }
        

    }
}