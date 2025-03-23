using Entities;
using Managers;
using SkillBridge.Message;
using UnityEngine;

public class EntityController : MonoBehaviour,IEntityNotify
{

    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 direction;
    Quaternion rotation;

    public UnityEngine.Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    // Use this for initialization
    void Start () {
        if (entity != null)
        {
            EntityManager.Instance.RegisterEnityChangeNotify(entity.entityId, this);//注册了对实体变化的监听。

            this.UpdateTransform();
        }

        if (!this.isPlayer)
            rb.useGravity = false;
    }

    void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rb.MovePosition(this.position);
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }
	
    void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);

        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    // Update is called once per frame
    void FixedUpdate()//更新实体状态
    {
        if (this.entity == null)
            return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if (!this.isPlayer)
        {
            this.UpdateTransform();//若当前角色不是本地玩家，会调用 UpdateTransform 方法更新角色的位置和方向：
        }
    }
    private bool isDestroyed = false;

    public void OnEntityRemoved()
    {
        if (isDestroyed)
        {
            return;
        }

        // 检查 UIWorldElementManager 实例是否为 null
        if (UIWorldElementManager.Instance != null)
        {
            // 可以在这里添加对 UIWorldElementManager 实例其他状态的检查
            // 例如检查它是否已经初始化完成等，这里简单以示例展示
            // 假设 UIWorldElementManager 有一个 IsInitialized 属性表示是否初始化完成
            // if (UIWorldElementManager.Instance.IsInitialized)
            {
                UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
            }
        }

        // 销毁当前游戏对象
        Destroy(this.gameObject);
        isDestroyed = true;

        // 移除不必要的异常抛出
        // throw new System.NotImplementedException();
    }

    public void OnEntityEvent(EntityEvent entityEvent)//接收到实体事件（如移动、跳跃等）时
    {
        switch(entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }

    public void OnEntityChaged(Entity entity)
    {
        Debug.LogFormat("OnEntityChaged: ID:{0} POS:{1} DIR:{2} SPD:{3}", entity.entityId, entity.position, entity.direction, entity.speed);
    }
}
