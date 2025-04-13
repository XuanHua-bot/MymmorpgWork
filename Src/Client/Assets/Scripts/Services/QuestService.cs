using System;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Networking;

namespace Services
{
    public class QuestService: Singleton<QuestService>,IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccpet);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccpet);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

       

        public bool SendQuestAccept(Quest quest) //发送 接受任务请求
        {
            Debug.Log(("SendQuestAccept"));
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestAccpet(object sender, QuestAcceptResponse message)// 接受任务请求
        {
            Debug.LogFormat("OnQuestAccept:{0},Err:{1}",message.Result,message.Erromsg);
            if (message.Result == Result.Success)
            {
                //把 请求的任务 提交给 客户端的任务管理器
                QuestManager.Instance.OnQuestAccepted(message.Quest);//任务管理器接受 服务器返回的任务信息
            }
            else
            {
                MessageBox.Show("任务接受失败喵", "错误", MessageBoxType.Error);
            }
        }

        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestSubmit");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }
        
        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("OnQuestSubmit:{0},Err:{1}",message.Result,message.Errormsg);
            if (message.Result==Result.Success)
            {
                //把 请求的任务 提交给 客户端的任务管理器
                QuestManager.Instance.OnQuestSubmited(message.Quest);
            }
            else
            {
                MessageBox.Show("任务提交失败喵", "错误", MessageBoxType.Error);

            }
        }
    }
}