﻿using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace GameServer.Services
{
    class QuestService: Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccpet);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);
        }

        public void Init()
        {   

        }
        
        private void OnQuestAccpet(NetConnection<NetSession>  sender, QuestAcceptRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestAcceptRequest::character :{0} : QuestId{1}",character.Id,request.QuestId);

            sender.Session.Response.questAccept = new QuestAcceptResponse();

            //把 请求的任务 提交给 服务器的任务管理器
            Result result = character.QuestManager.AcceptQuest(sender, request.QuestId);
            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();

        }
        
        //交任务请求
        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("QuestSubmitRequest :: character: {0} ：QuestId{1}",character.Id,request.QuestId);

            sender.Session.Response.questSubmit = new QuestSubmitResponse();
            
            //把 请求的任务 提交给 服务器的任务管理器
            Result result = character.QuestManager.SubmitQuest(sender, request.QuestId);
            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();
            
        }
    }
}