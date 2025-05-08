using Common;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGuildInfo : MonoBehaviour {

    public TMP_Text guildName;
    public TMP_Text guildID;
    public TMP_Text leader;

    public TMP_Text notice;

    public TMP_Text memberNumber;

    private NGuildInfo info;
    public NGuildInfo Info
    {
        get { return this.info; }
        set { this.info = value; this.UpdateUI(); }
    }

    void UpdateUI()
    {
        if (this.info == null)
        {
            this.guildName.text = "无";
            this.guildID.text = "ID:0";
            this.leader.text = "会长:无";
            this.notice.text = "";
            this.memberNumber.text = string.Format("成员数量 :0/{0}", GameDefine.GuildMaxMemberCount);
        }
        else
        {
            this.guildName.text = this.Info.GuildName;
            this.guildID.text = "ID:" + this.Info.Id;
            this.leader.text = "会长:" + this.Info.leaderName;
            this.notice.text = this.Info.Notice;
            this.memberNumber.text = string.Format("成员数量 :{0}/{1}",this.Info.memberCount,GameDefine.GuildMaxMemberCount);
        }
    }
}
