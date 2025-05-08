using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class GuildService : Singleton<GuildService>,IDisposable
    {
        public UnityAction OnGuildUpdate;//工会更新事件
        public UnityAction<bool> OnGuildCreatResult;//创建事件

        public UnityAction<List<NGuildInfo>> OnGuildListResult;//工会列表 请求更新的事件

        public  void Init()
        {

        }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreat);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

       
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreat);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

       




        /// <summary>
        /// 发送创建工会
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="notice"></param>
        public void SendGuildCreate(string guildName,string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guilCreate = new GuildCreateRequest();
            message.Request.guilCreate.GuildName = guildName;
            message.Request.guilCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);

        }

        /// <summary>
        /// 收到工会创建相应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildCreat(object sender, GuildCreateResponse response)//保证创建成功才会关掉ui
        {
            Debug.LogFormat("GuildCreateResponse:{0}", response.Result);

            if (OnGuildCreatResult!=null) //如果 有人订阅了OnGuildCreatResult 事件
            {
                this.OnGuildCreatResult(response.Result == Result.Success);
            }
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0} 工会创建成功", response.guildInfo.GuildName), "工会");
            }
            else
                MessageBox.Show(string.Format("{0} 工会创建失败", response.guildInfo.GuildName), "工会");
        }


        /// <summary>
        /// 发送加入工会请求
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest,申请人："+guildId);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);

        }
        
        /// <summary>
        /// 发送 审批加入请求（会长 or 管理
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendGuildJoinResponse(bool accept,GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;         
            message.Request.guildJoinRes.Apply.Result = request.Apply.Result;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
            

        }

        /// <summary>
        /// 收到加入工会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name), "工会申请", MessageBoxType.Confirm);
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
               this.SendGuildJoinResponse(false,request);
            };
        }


        /// <summary>
        /// 收到加入公会相应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("GuildJoinResponse 玩家加入工会请求结果: {0}",response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show("加入工会成功", "工会");
                
            }
            else
            {
                MessageBox.Show("工会拒绝了你的请求", "工会");
            }
        }


        /// <summary>
        /// 所属工会的详情 刷新 有任何信息变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild:{0} {1} {2}", message.Result, message.guildInfo.Id, message.Errormsg);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate!=null)
            {
                this.OnGuildUpdate();
            }
        }


        /// <summary>
        /// 发送离开工会请求
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendFriendRemoveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }


        /// <summary>
        /// 离开工会
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildLeave(object sender,GuildLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开工会成功", "工会");
            }
            else
            {
                MessageBox.Show("离开工会失败", "工会", MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 收到工会刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if (OnGuildListResult!=null)
            {
                this.OnGuildListResult(response.Guilds);
            }
        }



    }
}

