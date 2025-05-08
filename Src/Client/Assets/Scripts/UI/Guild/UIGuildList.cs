using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow {

    public GameObject itemPrefab;
    public ListView itemMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildItem selecetedItem;


    private void Start()
    {
        this.listMain.selecetedItem += this.OnGuildMemberSelected;//监听选中事件
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;//监听列表刷新

        GuildService.Instance.SendGuildListRequest();//发送列表请求 aka刷新

    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    public void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selecetedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selecetedItem.Info;
    }


    void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var item in guilds)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);

        }
    }

    void ClearList()
    {
        this.listMain.RemoveAll();
    }

    public void OnClickJoin()
    {
        if (selecetedItem == null)
        {
            MessageBox.Show("请选择要加入的工会");
            return;
        }

        MessageBox.Show(string.Format("确定要加入工会[{0}]吗 ？", selecetedItem.Info.GuildName, "申请加入工会", MessageBoxType.Confirm, "申请加入", "取消"));

    }
}
