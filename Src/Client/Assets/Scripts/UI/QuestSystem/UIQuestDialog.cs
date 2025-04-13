using System;
using Models;
using UnityEngine;

namespace UI.QuestSystem
{
    /// <summary>
    /// 任务对话框
    /// </summary>
    public class UIQuestDialog: UIWindow
    {
        public UIQuestInfo questInfo;

        public Quest quest; //当前打开的任务信息

        public GameObject openButtons;//接任务 拒绝任务
        public GameObject submitButtons;//提交任务




        public void SetQuest(Quest quest)
        {
            this.quest = quest;
            this.UpdateQuest();
            if (this.quest.Info == null)//判断服务器 该任务的info是否为空 空为新任务
            {
                openButtons.SetActive(true);
                submitButtons.SetActive(false);
            }
            else
            {
                if (this.quest.Info.Status ==SkillBridge.Message.QuestStatus.Complated)//判断任务状态
                {
                    openButtons.SetActive(true);
                    submitButtons.SetActive(false);
                }
                else
                {
                    openButtons.SetActive(false);
                    submitButtons.SetActive(false);
                }
            }
            
        }

        private void UpdateQuest()
        {
            if (this.quest!= null)
            {
                if (this.questInfo!=null)
                {
                   this.questInfo.SetQuestInfo(quest);
                }
            }
        }
    }
}