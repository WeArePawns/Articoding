using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    /* List of all tabs that should be managed */
    public List<TabButton> tabs;

    /* Current selected tab */
    private TabButton selectedTab;

    /* Associated PanelGroup */
    public PanelGroup panelGroup;

    public void Subscribe(TabButton tab)
    {
        if(tabs == null)
        {
            tabs = new List<TabButton>();
        }

        tabs.Add(tab);
    }

    public void OnSelected(TabButton tab)
    {
        selectedTab = tab;
        ResetTabs();

        /* Change to panel (order matters) */
        panelGroup.SetPanelIndex(tab.transform.GetSiblingIndex());
    }

    /* Deselect all tabs */
    private void ResetTabs()
    {
        foreach(TabButton tab in tabs)
        {
            tab.ResetTab();
        }
    }
}
