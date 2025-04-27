using System;
using Models;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class UITeam: MonoBehaviour
    {
        public TMP_Text teamTile;
        public UITeamItem[] Members;
        public ListView list;

        private void Start()
        {
            if (User.Instance.TeamInfo== null)//如果没有队伍 则关闭
            {
                this.gameObject.SetActive(false);
                return;
            }

            foreach (var item in Members)
            {
                this.list.AddItem(item);
            }
        }

        private void OnEnable()//组队界面打开时 更新ui
        {
            UpdateTeamUI();
        }

        public void ShowTeam(bool show)
        {
            this.gameObject.SetActive(show);
            if (show)
            {
                UpdateTeamUI();
            }
        }

        private void UpdateTeamUI()
        {
            if (User.Instance.TeamInfo == null)return;
            this.teamTile.text = string.Format("我的队伍({0}/5)", User.Instance.TeamInfo.Members.Count);

            for (int i = 0; i < 5; i++)
            {
                if (i<User.Instance.TeamInfo.Members.Count)
                {
                    //传入参数至UITeamItem 设置信息
                    this.Members[i].SetMenberInfo(i,User.Instance.TeamInfo.Members[i],User.Instance.TeamInfo.Members[i].Id==User.Instance.TeamInfo.Leader);
                    this.Members[i].gameObject.SetActive(true);
                }
                else
                {
                    this.Members[i].gameObject.SetActive(false);
                }
            }
        }

        public void OnClickLeave()
        {
            MessageBox.Show("确定要离开队伍吗？", "推出队伍", MessageBoxType.Confirm, "确定离开", "取消").OnYes = () =>
            {
                TeamService.Instance.SendTeamLeaveRequest(User.Instance.TeamInfo.Id);
            };
        }
    }
