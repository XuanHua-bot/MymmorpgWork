using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender, WindowResule result);
    public event CloseHandler Onclose;

    public virtual System.Type Type { get { return this.GetType(); } }

    public enum WindowResule
    {
        None = 0,
        Yes,
        No,
    }

    public void Close(WindowResule result = WindowResule.None)
    {
        UIManager.Instance.Close(this.Type); //调用ui管理器 执行 close 并执行事件 Onclose
        if (this.Onclose != null)
            this.Onclose(this, result);
        this.Onclose = null;

    }

    public virtual void OnCloseClick()
    {
        this.Close();
    }

    public virtual void OnYesClick()
    {
        this.Close(WindowResule.Yes);
    }
    public virtual void OnNoClick()
    {
        this.Close(WindowResule.No);
    }

    private void OnMouseDown() //点击测试
    {
        Debug.LogFormat(this.name + "Clicked//点击测试");

    }
}

