﻿using System;
using System.Collections.Generic;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class StatusManager
    {
        private Character Owner;
        private List<NStatus> Status { get; set; }

        public bool HasStatus
        {
            get { return this.Status.Count > 0; }
        }

        public StatusManager(Character owner)//构造函数，会创建一个新的 状态 列表
        {
            this.Owner = owner;
            this.Status = new List<NStatus>();
        }

        public void AddStatus(StatusType type, int id, int value, StatusAction action)
        {
            this.Status.Add(new NStatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });

        }

        public void AddGoldChange(int goldDelta) //金钱 增减方法
        {
            if (goldDelta>0)
            {
                this.AddStatus(StatusType.Money,0,goldDelta,StatusAction.Add);
            }

            if (goldDelta<0)
            {
                this.AddStatus(StatusType.Money,0,-goldDelta,StatusAction.Delete);
            }
        }

        public void AddItemChange(int id, int count, StatusAction action)// item 方法  action 的值有 update,add,delete
        {
            this.AddStatus(StatusType.Item, id, count, action);
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.statusNotify==null)
                message.statusNotify = new StatusNotify();
            foreach (var status in this.Status)
            {
                message.statusNotify.Status.Add(status);
            }
            this.Status.Clear();
        }
    }
}