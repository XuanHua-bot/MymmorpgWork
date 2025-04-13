using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using Models;
using Random = UnityEngine.Random;

public class NpcController : MonoBehaviour
{

    public int npcID;

    SkinnedMeshRenderer renderer;
    Animator anim;
    Color orignColor;

    private bool inInteractive = false; //是否处于交互  /是否处于点击

    NpcDefine npc;

    private NpcQuestStatus questStatus;

    // Use this for initialization
    void Start()
    {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();//有骨骼动画 所以从这里找渲染器
        anim = this.gameObject.GetComponent<Animator>();
        orignColor = renderer.sharedMaterial.color;
        npc = NPCManager.Instance.GetNpcDefine(npcID);
        this.StartCoroutine(Actions());//随机动作携程

        ReFreshNpcStatus();//刷新npc状态
        QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;// npc需要知道 任务状态有无改变
    }

    private void OnQuestStatusChanged(Quest quest)//任务改变了 则刷新 questStatus
    {
        this.ReFreshNpcStatus();
    }

    private void ReFreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);//获取任务管理器 中 有无 该npc id 对应的状态
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);//添加进uiWorldmanager 的状态
    }

    private void OnDestroy()//移除uiWorldmanager 的状态
    {
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
        }
    }


    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive)
            {
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(5f, 10f));
            }
            this.Relax();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Relax()
    {
        anim.SetTrigger("Relax");
    }

    void Interactive()//交互前判断
    {
        if (!inInteractive)//防止重复点击
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());//启动携程
        }
    }

    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();//面向玩家
        if (NPCManager.Instance.Interactive(npc))
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);
        inInteractive = false;
    }
    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            //Vector3.Lerp：线性插值，将当前正方向逐渐过渡到目标方向。
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }



    private void OnMouseDown()//交互触发
    {
        Interactive();
    }

    private void OnMouseOver()
    {
        HighLight(true);
    }
    private void OnMouseEnter()
    {
        HighLight(true);
    }
    private void OnMouseExit()
    {
        HighLight(false);
    }


    void HighLight(bool highLight)
    {
        if (highLight)
        {
            if (renderer.sharedMaterial.color != Color.white)
            {
                renderer.sharedMaterial.color = Color.white;
            }
            else
            {
                if (renderer.sharedMaterial.color != orignColor)
                    renderer.sharedMaterial.color = orignColor;
            }
        }
    }
}
