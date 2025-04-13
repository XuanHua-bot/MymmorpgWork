using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Models;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIQuestItem : ListView.ListViewItem
{

	//todo
	//ListView.ListViewItem 待补全
	public Text title;

	public Image background;
	public Sprite normalBg;
	public Sprite selectedBg;

	// Use this for initialization
	void Start () {
		
	}

	public override void onSelected(bool selected)
	{
		this.background.overrideSprite = selected ? selectedBg : normalBg;
	}

	public Quest quest;

	private bool isEquiped = false;
	public void SetQuestInfo(Quest item)
	{
		this.quest = item;
		if (this.title != null) this.title.text = this.quest.Define.Name;
		
	}
	// Update is called once per frame
	void Update () {
		
	}
}
