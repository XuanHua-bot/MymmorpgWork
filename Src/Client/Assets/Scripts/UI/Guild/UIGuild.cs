using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildMemberItem selectedItem;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;//工会信息更新  注册更新ui 方法
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate = null;
    }


    void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;//更新左侧info 面板 信息
        ClearList();
        InitItems();

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
            this.listMain.AddItem(ui);
        }
       
    }

    void ClearList()
    {
        this.listMain.RemoveAll();
    }

    public void OnClickAppliesList()
    {

    }
    public void OnClickLeave()
    {

    }
    public void OnClickChat()
    {

    }
    public void OnClickKickOut()
    {

    }
    public void OnClickPromote()
    { 
    

    }
    public void OnClickDespose()
    {

    }
    public void OnClickTransfer()
    {

    }
    public void OnClickNotice()
    {

    }


}
