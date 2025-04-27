using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Models;
using Services;
using UnityEngine;

public class UIFriends : UIWindow
{

	public GameObject itemPrefab;
	public ListView ListMain;
	public Transform itemRoot;
	public UIFriendItem selectedItem;

	private void Start()
	{
        FriendService.Instance.OnFriendUpdate = RefreshUI;
		this.ListMain.onItemSelected += this.OnFriendSelected;
		RefreshUI();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnFriendSelected(ListView.ListViewItem item)
	{
		this.selectedItem = item as UIFriendItem;

	}

	public void OnClickFriendAdd()
	{
		//绑定了even 点了提交就执行OnFriendAddSubmit
		InputBox.Show("请输入要添加的好友名称或ID","添加好友").OnSubmit += OnFriendAddSubmit;
	}

	private bool OnFriendAddSubmit(string input,out string tips)//out  传给调用方
	{
		tips = "";
		int friendId = 0;
		string FriendName = "";
		if (!int.TryParse(input,out friendId))//检测输入的时id还是 名字
			FriendName = input;
		if (friendId==User.Instance.CurrentCharacter.Id|| FriendName == User.Instance.CurrentCharacter.Name)
		{
			tips = "达令~ 不可以添加自己哦";
			return false;
		}

		FriendService.Instance.SendFriendAddRequest(friendId, FriendName);
		return true;

	}

	public void OnClickFriendChat()//聊天
	{
		MessageBox.Show("暂未开放");
	}

	public void onClickFriendTeamInvite()
	{
		if (selectedItem == null)
		{
			MessageBox.Show("请选择要邀请的闺蜜");
			return;
		}

		if (selectedItem.Info.Status == 0)
		{
			MessageBox.Show("请选择在线的闺蜜");
			return;
		}

      
        MessageBox.Show(String.Format("确定要邀请好友 [{0}] 加入队伍吗？", selectedItem.Info.friendInfo.Name), "组队邀请",MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
		{
			TeamService.Instance.SendTeamInviteRequest(this.selectedItem.Info.friendInfo.Id ,
				this.selectedItem.Info.friendInfo.Name);
		};
	}

	public void OnClickFriendRemove()
	{
		if (selectedItem==null)
		{
			MessageBox.Show("请选择要删除的好友");
			return;
		}

		MessageBox.Show(string.Format("确定要删除好友[{0}]吗？", selectedItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () => {
			FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id,this.selectedItem.Info.friendInfo.Id);
		};
	}

	void RefreshUI()
	{
		ClearFriendList();
		InitFriendItems();
	}

	

	/// <summary>
	/// 初始化好友列表
	/// </summary>
	void InitFriendItems()//遍历好友管理器列表  并实例化item 
	{
		foreach (var item in FriendManager.Instance.allFriends)
		{
			GameObject go = Instantiate(itemPrefab, this.ListMain.transform);

			UIFriendItem ui = go.GetComponent<UIFriendItem>();
			ui.SetFriendInfo(item);
			this.ListMain.AddItem(ui);
		}
	}
	
	
	private void ClearFriendList()
	{
		this.ListMain.RemoveAll();
	}
}
