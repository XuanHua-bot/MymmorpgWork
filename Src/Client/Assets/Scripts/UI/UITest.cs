﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UITest : UIWindow {
    //只写与业务有关的逻辑
    public Text title;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetTitle(string title)
    {
        this.title.text = title;
    }
}
