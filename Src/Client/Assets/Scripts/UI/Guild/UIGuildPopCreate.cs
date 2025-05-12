using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow {

    public InputField inputName;
    public InputField inputNotice;


    private void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated; //监听 工会创建结果  的事件
    }

    public void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;

    }

    public override void OnYesClick()//点击yes时不可以 关闭 ui   因为可能会出现创建失败   需要保留信息
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("请输入公会名称", "错误", MessageBoxType.Error);
            return;
        }
        if (inputName.text.Length<4 || inputName.text.Length>10)
        {
            MessageBox.Show("工会名称为4-10个字符", "错误", MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(inputNotice.text))
        {
            MessageBox.Show("请输入工会宣言", "错误", MessageBoxType.Error);
            return;
        }
        if (inputNotice.text.Length < 3 || inputNotice.text.Length > 50)
        {
            MessageBox.Show("工会宣言为3-50个字符", "错误", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(inputName.text, inputNotice.text);//发送协议
    }


    void OnGuildCreated(bool result)// 监听到 工会创建的事件了
    {
        if (result)
        {
            this.Close(WindowResule.Yes);//关闭窗口
        }
    }
}
