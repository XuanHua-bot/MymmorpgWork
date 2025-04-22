using Models;
using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using System.Linq;
using UI.QuestSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    
    public enum NpcQuestStatus
    {
        None = 0,//无任务
        Complete,//有已完成可提交的任务
        Available,//有可接受的任务
        Incomplete,//有未完成的任务
    }

    class QuestManager : Singleton<QuestManager>
    {
        /// <summary>
        /// =====NQuestInfo=====服务器 任务信息  包括id id 完成状态 目标=====
        /// </summary>
        public List<NQuestInfo> questInfos;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();//储存所有任务

        //保存npc 身上的任务 <npcid           <任务状态枚举    任务信息>>
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> onQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)//角色登陆后执行  进行任务管理器的初始化
        {
           //？ 清空服务器的信息吗
            this.questInfos = quests;
            
            //清空本地信息
            allQuests.Clear();
            this.npcQuests.Clear();
            
            //任务初始化
            InitQuests();
        }

        void InitQuests()
        {
            //初始化已有任务           
            foreach (var info in this.questInfos)//查询 服务端 已有任务
            {
                //创建本地models = 服务器 信息
                Quest quest = new Quest(info);
               
                this.allQuests[quest.Info.QuestId] = quest;
            }

            this.CheckAvailableQuests();//查询 可接任务

            foreach (var kv in this.allQuests)//遍历 所有任务  加到npc上
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        //初始化可用任务
        void CheckAvailableQuests()
        {
            foreach (var kv in DataManager.Instance.Quests)//读表 根据条件  查询 玩家 可接任务
            {
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                    continue;//不符合职业

                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue;//不符合等级

                if (this.allQuests.ContainsKey(kv.Key))
                    continue;//任务已存在

                if (kv.Value.PreQuest > 0)//查询前置任务 是否完成 
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))//获取前置任务
                    {
                        if (preQuest.Info == null)
                            continue;//前置任务未接取
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue;//前置任务未完成
                    }
                    else
                        continue;//前置任务还没接
                }
                Quest quest = new Quest(kv.Value);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        void AddNpcQuest(int npcId, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId))//任务加过没
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            //npc 上的 任务列表
            List<Quest> availables;
            List<Quest> complates;
            List<Quest> incomplates;

            //↓↓  分配任务 到npc的 三个 任务状态列表 ↓↓
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incomplates;
            }

            if (quest.Info  == null)
            {
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }
            }
        }

        /// <summary>
        /// 获取NPC任务状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)//查询npc身上有无任务状态
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))//获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)//打开npc对话框
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))//获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        bool ShowQuestDialog(Quest quest)//接取任务后 再次点击npc 对话框的显示逻辑
        {
            //如果 任务没接过  或者 任务的状态是已完成
            //创建对话框
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQuest(quest);//设置  面板内 该显示哪些按钮
                
                //执行uiwindow的void Close（）   会传入Yes 或No
                dlg.Onclose += OnQuestDialogClose;//点击了关闭  执行  
                return true;
            }
            
            //没完成就不会创建 对话
            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        //任务对话框关闭后
        void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResule result)
        {
            // 检查 sender 是否为空
            if (sender == null)
            {
                Debug.LogError("sender is null in OnQuestDialogClose");
                return;
            }

            UIQuestDialog dlg = (UIQuestDialog)sender;

            // 检查 dlg.quest 是否为空
            if (dlg.quest == null)
            {
                Debug.LogError("dlg.quest is null in OnQuestDialogClose");
                return;
            }

            //任务对话框点了 Yes
            if (result == UIWindow.WindowResule.Yes)
            {
                if (dlg.quest.Info == null)
                {
                    QuestService.Instance.SendQuestAccept(dlg.quest);//发送 接受任务 的协议
                }
                else if (dlg.quest.Info.Status == QuestStatus.Complated) // 如果 该任务已经完成了
                {
                    QuestService.Instance.SendQuestSubmit(dlg.quest);//发送 提交任务 的协议
                }
            }
            //任务对话框点了 No
            else if (result == UIWindow.WindowResule.No)
            {
                // 检查 dlg.quest.Define 是否为空
                if (dlg.quest.Define == null)
                {
                    Debug.LogError("dlg.quest.Define is null in OnQuestDialogClose");
                    return;
                }
                MessageBox.Show(dlg.quest.Define.DialogDeny);
            }
        }

        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            this.npcQuests.Clear();
            Quest result;
            if (this.allQuests.ContainsKey(quest.QuestId))//遍历 任务列表  并把已有任务添加到任务列表 
            {
                //更新新的任务状态
                this.allQuests[quest.QuestId].Info = quest;//如果没有新的任务 则把服务器发来的同步到本地
                result = this.allQuests[quest.QuestId];
            }
            else//把新任务添加 进人物列表
            {
                result = new Quest(quest);
                this.allQuests[quest.QuestId] = result;
            }

            CheckAvailableQuests();
            //把任务列表添加到 npc任务列表上

            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            if (onQuestStatusChanged != null)//任务状态通知  npccontroller 订阅的 刷新状态
                onQuestStatusChanged(result);
            return result;
        }

        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
