using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject nameBarPrefab;

    private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

    // Use this for initialization
    protected override void OnStart()
    {
        nameBarPrefab.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddCharacterNameBar(Transform owner,Character character)
    {
        if (owner != null)
        {
            //它实例化一个名字条预制体，并将其设置为 UIWorldElementManager 的子对象。
            GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);

            //设置名字条的名称、所有者（owner）和角色信息（character）
            goNameBar.name = "NameBar " + character.entityId;
            goNameBar.GetComponent<UIWorldElement>().owner = owner;
            goNameBar.GetComponent<UINameBar>().character = character;
            goNameBar.SetActive(true);
            //将其添加到 elements 字典中，以便后续管理。
            this.elements[owner] = goNameBar;
        }
        else
        {
            Debug.LogError("Owner is null, cannot add character name bar.");
        }
        
    }
    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elements.ContainsKey(owner) )
        {
            Destroy(this.elements[owner]);
            this.elements.Remove(owner);
        }
       
    }
}
