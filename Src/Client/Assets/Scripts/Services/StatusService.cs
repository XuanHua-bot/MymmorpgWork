using System;
using System.Collections.Generic;
using System.IO;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    public class StatusService:Singleton<StatusService>,IDisposable
    {
        //接收一个 NStatus 类型的参数，并返回一个布尔值。
        public delegate bool StatusNotifyHanlder(NStatus status);
        
        //键为 StatusType 枚举类型，值为 StatusNotifyHandler 委托。
        Dictionary<StatusType,StatusNotifyHanlder> eventMap = new Dictionary<StatusType, StatusNotifyHanlder>();
        private HashSet<StatusNotifyHanlder> hanlders = new HashSet<StatusNotifyHanlder>();
        
        public void Init()
        {
            
        }

        //状态注册通知
        public void RegisterStatusNotify(StatusType function, StatusNotifyHanlder action)
        {
            if (hanlders.Contains(action))
            {
                return;
            }
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }

            hanlders.Add(action);
        }

        public StatusService()//构造函数
        {
            //监听状态协议
            //使用 MessageDistributer 订阅 StatusNotify 消息，并指定处理方法为 OnStatusNotify。
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }
        
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

        private void OnStatusNotify(object sender, StatusNotify notify)
        {
            //遍历一下状态的协议
            foreach (NStatus status in notify.Status)
            {
                Notify(status);
            }
        }

        private void Notify(NStatus status)
        {
            Debug.LogFormat("StatusNotify:[{0}] [{1}] {2}:{3}", status.Type, status.Action, status.Id, status.Value);

            if (status.Type == StatusType.Money)
            {
                if (status.Action == StatusAction.Add)
                {
                    User.Instance.AddGold(status.Value);
                }
                else if (status.Action == StatusAction.Delete)
                {
                    User.Instance.AddGold(-status.Value);
                }
            }
            StatusNotifyHanlder handler; //谁在注册其他消息
            if (eventMap.TryGetValue(status.Type,out handler))
            {
                handler(status);
            }
        }
        
        
    }
}