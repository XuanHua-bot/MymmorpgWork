using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {

        public NGuildInfo guildInfo;//当前工会信息
        public NGuildMemberInfo myMemberInfo;//自己的成员信息（职位、等级、、）
        public bool HasGuild//有无工会
        {
            get { return this.guildInfo != null; }
        }

        public void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
            if (guild == null)//如果没有 工会  那就没有成员信息
            {
                myMemberInfo = null;
                return;
            }
            foreach(var mem in guild.Members)//遍历成员，查询自己的信息 并赋值
            {
                if (mem.Characterid == User.Instance.CurrentCharacter.Id)
                {
                    myMemberInfo = mem;
                    break;
                }
            }
        }

        public void ShowGuild()
        {
            if (this.HasGuild) //判断有无工会 
            {
                var win = UIManager.Instance.Show<UIGuild>();//弹出 加入的工会ui
            }
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();//弹出 加入  or 创建

                win.Onclose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResule resule)//判断 加入  or 创建
        {
            if (resule == UIWindow.WindowResule.Yes)
            {//创建公会
                UIManager.Instance.Show<UIGuildPopCreate>();//创建工会的ui
            }
            else if (resule == UIWindow.WindowResule.No)
            {//加入工会
                UIManager.Instance.Show<UIGuildList>();//加入工会的ui
            }
        }
    }

}
