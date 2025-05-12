using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildMemberItem selectedItem;

    public GameObject panelAdmin;
    public GameObject panelLeader;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;//工会信息更新  注册更新ui 方法
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }


    void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;//更新左侧info 面板 信息
       
        ClearList();
        InitItems();

        this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);//有管理权限
        this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
    }

    public void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            this.listMain.AddItem(ui);
        }
       
    }

    void ClearList()
    {
        this.listMain.RemoveAll();
    }

    public void OnClickAppliesList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }
    public void OnClickLeave()
    {
        MessageBox.Show("扩展作业");
    }
    public void OnClickChat()
    {

    }
    public void OnClickKickOut()
    {
        if (selectedItem==null)
        {
            MessageBox.Show("请选择要提出的成员");
            return;
        }
        MessageBox.Show(string.Format("要把{0}提出工会吗",selectedItem.Info.Info.Name),"提出工会",MessageBoxType.Confirm).OnYes=()=>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kikcout, this.selectedItem.Info.Info.Id);
        } ;
    }
    public void OnClickPromote()
    {

        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的成员");
            return;
        }
        if (selectedItem.Info.Title != GuildTitle.None)
        {
            MessageBox.Show("无法提升对象的身份");
            return;
        }
        MessageBox.Show(string.Format("确定要晋升成员 {0} 为副会长吗？", selectedItem.Info.Info.Name), "晋升", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Id);
        };


    }
    public void OnClickDespose()
    {
        if (selectedItem ==null)
        {
            MessageBox.Show("请选择要罢免的成员");
            return;
        }
        if (selectedItem.Info.Title!=GuildTitle.None)
        {
            MessageBox.Show("对方已是普通成员");
            return;
        }
        if (selectedItem.Info.Title!=GuildTitle.President)
        {
            MessageBox.Show("无法罢免会长");
            return;
        }
        MessageBox.Show(string.Format("确定要罢免成员 {0} 吗？",selectedItem.Info.Info.Name), "罢免", MessageBoxType.Confirm).OnYes = () => 
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depose, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickTransfer()
    {
        if (selectedItem==null)
        {
            MessageBox.Show("请选择要转让的成员");
            return;
        }
        else if (selectedItem.Info.Id==User.Instance.CurrentCharacter.Id)
        {
            MessageBox.Show("需要选择其他工会会员");
            return;
        }
        MessageBox.Show(string.Format("确定要转让会长给 {0} 吗？", selectedItem.Info.Info.Id), "转让会长", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickNotice()
    {
        MessageBox.Show("扩展作业");
    }


}
