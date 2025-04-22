using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Services;
using SkillBridge.Message;
using Managers;
using Models;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{
    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();//用于存储游戏中的所有角色对象

    // Use this for initialization
    protected override void OnStart()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    private void OnDestroy()//在 GameObjectManager 被销毁时，取消订阅 
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCharacterEnter(Character cha)//当有新角色进入游戏时，调用 CreateCharacterObject 方法为该角色创建游戏对象。
    {
        CreateCharacterObject(cha);
    }

    private void OnCharacterLeave(Character character)
    {
        if (!Characters.ContainsKey(character.entityId))
            return;
        if (Characters[character.entityId] != null)
        {
            Destroy(Characters[character.entityId]);
            this.Characters.Remove(character.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    private void CreateCharacterObject(Character character)
    {
        if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
        {
            // 加载角色资源并创建游戏对象
            UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(character.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj, this.transform);
            go.name = "Character_" + character.Id + "_" + character.Info.Name;

            Characters[character.entityId] = go;//将游戏对象存储到 Characters 字典中

            // 初始化角色对象
            InitGameObject(go, character);
        }
    }

    private void InitGameObject(GameObject go, Character character)//会根据接收到的角色信息初始化游戏对象：
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
        }

        PlayerInputController pc = go.GetComponent<PlayerInputController>();
        if (pc != null)
        {
            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = go;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityController = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }
        //todo 修改了uiworldElement 可能会报错
        //UIWorldElementManager.Instance.AddCharacterNameBar(go.transform,character);
    }
}