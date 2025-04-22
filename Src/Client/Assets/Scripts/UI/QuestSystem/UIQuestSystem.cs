using System;
using System.Net.Mime;
using Common.Data;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestSystem
{
    public class UIQuestSystem: UIWindow
    {
        public Text title;
        public GameObject itemPrefab;

        public TabView Tabs;
        
        [Header("主线任务")]
        public ListView ListMain;//主线
        [Header("支线任务")]
        public ListView listBranch;//支线

        public UIQuestInfo questInfo;

        private bool showAvailableList = false;//是否显示可接任务


        private void Start()
        {
            //注册 方法
            this.ListMain.onItemSelected += this.OnQuestSelected;
            this.listBranch.onItemSelected += this.OnQuestSelected;
            this.Tabs.OnTabSelect += OnSelectTab;  //可接任务列表 已接任务列表
            //todo
            //需补全
            RefreshUI();
            
        }

        void OnSelectTab(int idx)
        {
            //已接任务列表 =0
            //可接任务列表 =1
            showAvailableList = idx == 1;//  true or false
            RefreshUI();
        }

        private void OnDestroy()
        {
            
        }

        void RefreshUI()
        {
            ClearAllQuestLIST();
            InitAllQuestItems();
        }

       

        void InitAllQuestItems()
        {
            foreach (var kv in QuestManager.Instance.allQuests)// 任务管理器中所有可用任务  可接已接
            {
                if (showAvailableList)
                {
                    
                    if (kv.Value.Info!= null)
                         continue;
                }
                else
                {
                    if (kv.Value.Info == null)// 在网络信息 中没有  则是未接任务
                        continue;
                }

                //实例化                                 类型是否为 主线        是 则实例化到main的父节点  否则支线
                GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.ListMain.transform : this.listBranch.transform);
                
                //获取实例化后的 任务列表 元素 的 uiquestitem 脚本
                UIQuestItem ui = go.GetComponent<UIQuestItem>();
                
                //设置任务元素 UIQuestItem 的任务类型 SetQuestInfo(Quest item)
                ui.SetQuestInfo(kv.Value);


                /*if (kv.Value.Define.Type == QuestType.Main)
                {
                    //元素为主线的  放到 主线list
                    this.ListMain.AddItem(ui);
                }
                else
                {
                    //为支线的 放到 支线list
                    this.listBranch.AddItem(ui);
                }*/
                this.ListMain.AddItem(ui);
            }
        }
        
        private void ClearAllQuestLIST()
        {
            this.ListMain.RemoveAll();
            this.listBranch.RemoveAll();
        }
        
        public void OnQuestSelected(ListView.ListViewItem item)
        {
            //  分支列表 选择的物体不为空  且  请求选择物体的owner不为 分支列表
            if (listBranch.SelectedItem!=null && item.owner!=listBranch)
            {
                listBranch.SelectedItem.onSelected(false);
            }
            else if (ListMain.SelectedItem!=null && item.owner!= ListMain)
            {
                ListMain.SelectedItem.onSelected(false);
            }
           

           
            UIQuestItem questItem = item as UIQuestItem;
            
            this.questInfo.SetQuestInfo(questItem.quest);
        }

        
    }
}