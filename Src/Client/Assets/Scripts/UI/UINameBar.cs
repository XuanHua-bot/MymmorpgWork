using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UINameBar : MonoBehaviour
{

    //public Image avatar;
    public Text characterName;
    public Character character;
	// Use this for initialization
	void Start () {

       /*if (character!=null)
        {
            if (character.Info.Type == SkillBridge.Message.CharacterType.Monster)
            {
                this.avatar.gameObject.SetActive(false );
            }
            else
            {
                this.avatar.gameObject.SetActive(true);
            }
        }*/
	}
	
	// Update is called once per frame
	void Update () {
        this.UpdateInfo();

        //this.transform.forward= Camera.main.transform.forward;//设置 nameBar朝向
    }
    void UpdateInfo()
    {
        if (this.character!=null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if (name!=this.characterName.text)//更新人物名称
            {
                this.characterName.text = name;
            }
        }
    }
}
