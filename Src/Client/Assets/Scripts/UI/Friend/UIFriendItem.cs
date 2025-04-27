using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem
{
    public TMP_Text nickid;
    public TMP_Text nickname;
    public TMP_Text @class;
    public TMP_Text level;
    public TMP_Text status;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

	public NFriendInfo Info;


	private bool isEquiped = false;

    public void SetFriendInfo(NFriendInfo item)
    {
        this.Info = item;
        if (this.nickname != null) this.nickname.text = this.Info.friendInfo.Name;
        if (this.@class != null) this.@class.text = this.Info.friendInfo.Class.ToString();
        if (this.level != null) this.level.text = this.Info.friendInfo.Level.ToString();
        if (this.status != null) this.status.text = this.Info.Status == 1 ? "在线" : "离线";
        if (this.nickid!=null) this.nickid.text = this.Info.friendInfo.Id.ToString();
       
        if (this.status != null)
        {
            // 根据在线状态设置颜色
            this.status.color = this.Info.Status == 1 ? Color.green : Color.red;
            
        }
    }
}
