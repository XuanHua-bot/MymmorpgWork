using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour {

    public TabButton[] tabButtons;
    public GameObject[] tabPages;

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
                tabPages[i].SetActive(i == index);
            }
        }
    }

	
	
	// Update is called once per frame
	void Update () {
		
	}
}
