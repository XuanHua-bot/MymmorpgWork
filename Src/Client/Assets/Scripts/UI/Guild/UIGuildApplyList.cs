using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;

    private void Start()
    {
        //aka 强制刷新
        GuildService.Instance.OnGuildUpdate += UpdateList;//打开时 注册 更新的方法
        GuildService.Instance.SendGuildListRequest();//每次打开时发送 列表请求
        this.UpdateList();

    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }
    private void UpdateList()
    {
        ClearList();
        InitItems();

    }

    void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetItemInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    void ClearList()
    {
        this.listMain.RemoveAll();
    }
}
