using System.Collections.Generic;
using System.Linq;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace GameServer.Managers
{
    class QuestManager
    {
        private Character Owner;

        public QuestManager(Character owner)
        {
            this.Owner = owner;
        }

        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach (var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        public NQuestInfo GetQuestInfo(TCharacterQuest quest) //使用数据库数据 构造一份网络数据
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
                Status = (QuestStatus)quest.Status,
                Targets = new int[3]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };


        }

        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character; //获取当前角色

            QuestDefine quest;
            if (DataManager.Instance.Quest.TryGetValue(questId, out quest))
            {
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = quest.ID;
                if (quest.Target1 == QuestTarget.None)
                {
                    //没有任务目标 直接完成
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else
                {
                    //有目标则进行中
                    dbquest.Status = (int)QuestStatus.InProgress;
                }

                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest); //db数据转换为网络数据
                character.Data.Quests.Add(dbquest); //添加到角色身上
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Erromsg = "任务不存在喵";
                return Result.Failed;
            }
        }

        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character; //获取当前角色

            QuestDefine quest;
            if (DataManager.Instance.Quest.TryGetValue(questId, out quest))
            {
                //                                                                         表中 有没有一个任务id 等于请求id  没有则返回默认 默认为空
                var dbquest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (dbquest != null)
                {
                    //判断是否处于完成
                    if (dbquest.Status != (int)QuestStatus.Complated)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "当前任务未完成喵";
                        return Result.Failed;
                    }
                    dbquest.Status = (int)QuestStatus.Finished;
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    DBService.Instance.Save();

                    //任务奖励
                    if (quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }

                    if (quest.RewardExp > 0)
                    {
                        //character.Exp += quest.RewardExp;
                        Debug.Log("经验增加啦~  PS:功能未实现 ");
                    }

                    if (quest.RewardItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                    }

                    if (quest.RewardItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                    }

                    if (quest.RewardItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                    }

                    DBService.Instance.Save();
                    return Result.Success;
                }
                else
                {
                    sender.Session.Response.questAccept.Erromsg = "任务不存在[2]";
                    return Result.Failed;
                }
            }
            else
            {
                sender.Session.Response.questAccept.Erromsg = "任务不存在[1]";
                return Result.Failed;
            }
        }
    }
}