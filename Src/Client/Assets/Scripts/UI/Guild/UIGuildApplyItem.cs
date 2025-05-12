using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGuildApplyItem : ListView.ListViewItem {
    //没有选中 项   直接在unity中进行绑定

    public TMP_Text nickname;
    public TMP_Text @class;
    public TMP_Text level;

    public NGuildApplyInfo Info;

    public void SetItemInfo(NGuildApplyInfo item)
    {
        this.Info = item;
        if (this.nickname != null) this.nickname.text = this.Info.Name;
        if (this.@class != null) this.@class.text = this.Info.Class.ToString();
        if (this.level != null) this.level.text = this.Info.Level.ToString();
    }

    public void OnAccept()
    {
        MessageBox.Show(string.Format("要通过[{0}]的工会申请吗？", this.Info.Name), "审批申请", MessageBoxType.Confirm,"同意","取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }

    public void OnDecline()
    {
        MessageBox.Show(string.Format("要拒绝[{0}]的工会申请吗？", this.Info.Name), "审批申请", MessageBoxType.Confirm,"拒绝", "取消").OnNo = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
