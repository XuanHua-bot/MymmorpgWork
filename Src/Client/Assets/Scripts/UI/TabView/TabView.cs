using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour {

    public TabButton[] tabButtons;
    public GameObject[] tabPages;

    public UnityAction<int> OnTabSelect;

    public int index = -1;

    IEnumerator Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;//给每个按钮指定所有者
            tabButtons[i].tabIndex = i;

        }
        yield return new WaitForEndOfFrame();
        SelectTab(0);//默认选择第一页
    }

    public void SelectTab(int index)
    {
        if (this.index!=index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);
                //tabPages[i].SetActive(i == index);
                if (i < tabPages.Length - 1)
                    tabPages[i].SetActive(i == index);
            }
            if (OnTabSelect != null)
                OnTabSelect(index);
        }
    }

	
	
	// Update is called once per frame
	void Update () {
		
	}
}
