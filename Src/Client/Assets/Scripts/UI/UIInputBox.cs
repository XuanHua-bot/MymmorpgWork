﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour
{


    public TMP_Text title;
    public TMP_Text message;
    public TMP_Text tips;
    public Button buttonYes;
    public Button buttonNo;
    public InputField input;
    

    public TMP_Text buttonYesTitle;
    public TMP_Text buttonNoTitle;

    public delegate bool SubmitHandler(string inputText, out string tips);
    public event SubmitHandler OnSubmit;
    public UnityAction OnCancel;

    public string emptyTips;

    
    


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(string title, string message,string btnOK = "", string btnCancel = "",string emptyTips="")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message;
        this.tips.text = null;
        this.OnSubmit = null;
        this.emptyTips = emptyTips;

        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = title;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = title;

        this.buttonYes.onClick.AddListener(OnClickYes);
        this.buttonNo.onClick.AddListener(OnClickNo);

        
    }

    void OnClickYes()
    {
        this.tips.text = "";
        if (string.IsNullOrEmpty(input.text))
        {
            this.tips.text = this.emptyTips;
            return;
        }
        if (OnSubmit !=null)
        {
            string tips;
            if(!OnSubmit(this.input.text,out tips))
            {
                this.tips.text = tips;
                return;
            }
        }
        Destroy(this.gameObject);
    }

    void OnClickNo()
    {
        Destroy(this.gameObject);
        if (this.OnCancel != null)
            this.OnCancel();
    }
}
