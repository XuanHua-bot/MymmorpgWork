using UnityEngine;


// 定义一个泛型抽象类 MonoSingleton，继承自 MonoBehaviour，用于实现单例模式
// 适用于 Unity 游戏开发，确保一个类只有一个实例，并提供全局访问点
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 全局标志，若为 true，则该单例对象在场景切换时不会被销毁
    public bool global = true;
    // 静态字段，用于存储单例实例
    static T instance;

    // 静态属性，用于获取单例实例
    public static T Instance
    {
        get
        {
            // 如果实例为空
            if (instance == null)
            {
                // 通过 FindObjectOfType 方法查找场景中第一个类型为 T 的组件实例
                // 这种方式确保了实例的懒加载，即只有在需要时才会创建实例
                instance = (T)FindObjectOfType<T>();
            }
            return instance;
        }
    }

    // 当对象启动时调用此方法
    void Start()
    {
        // 检查全局标志是否为 true
        if (global)
        {
            // 检查单例实例是否已经存在，并且当前对象上的 T 组件不是已存在的实例
            if (instance != null && instance != this.gameObject.GetComponent<T>())
            {
                // 如果发现已经存在一个单例实例，并且当前对象上的 T 组件不是该实例
                // 则销毁当前对象，以确保只有一个单例实例存在，避免逻辑错误
                Destroy(this.gameObject);
                return;
            }
            // 调用 DontDestroyOnLoad 方法，确保该对象在场景切换时不会被销毁
            DontDestroyOnLoad(this.gameObject);
            // 将当前对象上的 T 组件设置为单例实例
            instance = this.gameObject.GetComponent<T>();
        }

        // 调用抽象方法 OnStart，由子类实现具体逻辑
        this.OnStart();
    }

    // 受保护的虚方法，可由子类重写，用于在对象启动时执行特定逻辑
    protected virtual void OnStart()
    {

    }
}