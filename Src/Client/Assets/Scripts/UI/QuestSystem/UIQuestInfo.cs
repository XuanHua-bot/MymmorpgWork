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
                if (quest.Define.Target1 == null)
                {
                    targets[0].text = quest.Define.Overview;
                    targets[1].gameObject.SetActive(false);
                    targets[2].gameObject.SetActive(false);
                }
                else
                {
                    targets[0].gameObject.SetActive(true);
                    targets[1].gameObject.SetActive(true);
                    targets[2].gameObject.SetActive(true);
                    targets[0].text = quest.Define.Target1ID.ToString() + " X" + quest.Define.Target1Num.ToString();
                    if (quest.Define.Target2 != null)
                        targets[1].text = quest.Define.Target2ID.ToString() + " X" + quest.Define.Target2Num.ToString();
                    if (quest.Define.Target3 != null)
                        targets[2].text = quest.Define.Target3ID.ToString() + " X" + quest.Define.Target3Num.ToString();
                }
                this.rewardMoney.text = quest.Define.RewardGold.ToString();
                this.rewardExp.text = quest.Define.RewardExp.ToString();

            }
            else
            {

                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
                {
                    this.description.text = quest.Define.DialogFinish;
                }
            }


            this.description.text = quest.Define.Dialog;
            if (quest.Define.Target1 == null)
            {
                targets[0].text = quest.Define.Overview;
                targets[1].gameObject.SetActive(false);
                targets[2].gameObject.SetActive(false);
            }
            else
            {
                targets[0].gameObject.SetActive(true);
                targets[1].gameObject.SetActive(true);
                targets[2].gameObject.SetActive(true);
                targets[0].text = quest.Define.Target1ID.ToString() + " X" + quest.Define.Target1Num.ToString();
                if (quest.Define.Target2 != null)
                    targets[1].text = quest.Define.Target2ID.ToString() + " X" + quest.Define.Target2Num.ToString();
                if (quest.Define.Target3 != null)
                    targets[2].text = quest.Define.Target3ID.ToString() + " X" + quest.Define.Target3Num.ToString();
            }
            
            foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
            {
                fitter.SetLayoutVertical();//强制调用自适应布局
            }

        }

    }
}