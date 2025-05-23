﻿using Assets.Scripts.UI;
using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UI.QuestSystem;

using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain> {
    public Text avatarName;
    public Text avatarLevel;

    public UITeam TeamWindow;

	// Use this for initialization
	protected override void OnStart () {
        this.UpdateAvata();
    }
	
    void UpdateAvata()
    {
        this.avatarName.text =string.Format("{0} [{1}]" ,User.Instance.CurrentCharacter.Name,User.Instance.CurrentCharacter.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }
	// Update is called once per frame
	

    public void backToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    //测试用方法
    public void OnclickTest()
    {
       UITest test = UIManager.Instance.Show<UITest>();
       test.SetTitle("这是一个测试");
       test.Onclose += Test_OnClose;
    }

    private void Test_OnClose(UIWindow sender, UIWindow.WindowResule result)
    {
        MessageBox.Show("你关闭了 测试对话UI" + result, "对话框相应结果",MessageBoxType.Information);
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickCharEquip()
    {
        
        UIManager.Instance.Show<UICharEquip>();
    }
    
    public void OnClickQuest()//显示任务
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }

    public void OnClickGuild()
    {
        //工会管理器间接打开ui
        GuildManager.Instance.ShowGuild();
       
        
    }

    public void OnClickRide()
    {

    }

    public void OnClickSetting()
    {

    }

    public void OnClickSkill()
    {

    }


    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }
}
