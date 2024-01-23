using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<GameObject> tabContent;
    private List<TabScript> Tabs;
    public TabScript selectTab;
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;



    // Start is called before the first frame update
    public void ResetTabs()
    {
        foreach (TabScript btn in Tabs)
        {
            if (selectTab != null && btn == selectTab)
                continue;
            btn.background.color = tabIdle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Subscribe(TabScript btn)
    {
        if (Tabs == null)
        {
            Tabs = new List<TabScript>();
        }

        Tabs.Add(btn);
    }

    public void OnTabEnter(TabScript btn)
    {
        ResetTabs();
        if (selectTab != null || btn != selectTab)
        {
            btn.background.color = tabHover;

        }
    }


    public void OnTabExit(TabScript btn)
    {
        ResetTabs();
        btn.background.color = tabActive;

    }

    public void OnTabSelected(TabScript btn)
    {
        selectTab = btn;
        ResetTabs();
        btn.background.color = tabActive;
        int index = btn.transform.GetSiblingIndex();
        for (int i = 0; i < tabContent.Count; i++)
        {
            if (i==index)
            {
                tabContent[i].SetActive(true);
            }
            else
                tabContent[i].SetActive(false);

        }

    }

    public void OnTabHover(TabScript btn)
    {
        ResetTabs();

    }


}
