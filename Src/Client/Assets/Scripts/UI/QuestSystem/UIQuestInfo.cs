using System.Net.Mime;
using Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestSystem
{
    /// <summary>
    /// 任务详情 标题描述 奖励
    /// </summary>
    public class UIQuestInfo: MonoBehaviour
    {
        public Text title;
        public Text[] targets;
        public Text description;
        public UIIconItem rewardItems;
        public Text rewardMoney;
        public Text rewardExp;

        public void SetQuestInfo(Quest quest)
        {
            this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
            if (quest.Info == null)
            {
                this.description.text = quest.Define.Dialog;
            }
            else
            {
                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
                {
                    this.description.text = quest.Define.DialogFinish;
                }
            }

            this.rewardMoney.text = quest.Define.RewardGold.ToString();
            this.rewardExp.text = quest.Define.RewardExp.ToString();

            foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
            {
                fitter.SetLayoutVertical();//强制调用自适应布局
            }

        }

    }
}