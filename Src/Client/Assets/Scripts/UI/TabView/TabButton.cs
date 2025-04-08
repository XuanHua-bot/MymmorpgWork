using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour {

    //按钮 普通图片 和被激活时图片
    public Sprite activeImage;
    private Sprite normalImage;//在Start 自动获取

    public TabView tabView;

    public int tabIndex = 0; //按钮的索引 ID
    public bool selected = false;

    private Image tabImage;

    private void Start()
    {
        tabImage = this.GetComponent<Image>();
        normalImage = tabImage.sprite;//获取normalImage

        this.GetComponent<Button>().onClick.AddListener(OnClick);


    }


    public void Select(bool select)
    {
        if ( tabImage.overrideSprite!= null)
        {
            tabImage.overrideSprite = select ? activeImage : normalImage; //三元运算符
        }
        
    }

    void OnClick()
    {
        this.tabView.SelectTab(this.tabIndex);
    }
}
