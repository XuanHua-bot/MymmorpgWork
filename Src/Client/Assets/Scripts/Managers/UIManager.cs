using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

    
    //Class
    class UIElement
    {
        public string Resources; //资源路径
        public bool Cache;
        public GameObject Instance;
    }

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();//保存定义的ui信息
    public UIManager()//构造函数
    {
        //预先把 ui添加到ui管理器中  管理器才能管理 ui
        //测试用 ui
        this.UIResources.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest", Cache = true });
    }

    ~UIManager()
    {

    }

    /// <summary>
    /// Show UI
    /// </summary>
    
    public T Show<T>()
    {
        //SoundManager.Instance.PlaySound("ui_open");
        Type type = typeof(T);
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Instance!= null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close (Type type)
    {
        //SoundManager.Instance.PlaySound("ui_close");
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }

    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
