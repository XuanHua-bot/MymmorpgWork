using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoSingleton<UIMainCity> {
    public Text avatarName;
    public Text avatarLevel;

	// Use this for initialization
	protected override void OnStart () {
        this.UpdateAvata();
    }
	
    void UpdateAvata()
    {
        this.avatarName.text =string.Format("{0} [{1}]" ,User.Instance.CurrentCharacter.Name,User.Instance.CurrentCharacter.Id);
        this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }
	// Update is called once per frame
	

    public void backToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        UserService.Instance.SendGameLeave();
    }
}
