using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    private enum TabType
    {
        SHOP_TAB = 0,
        TEMARY_TAB,
        PLAY_TAB,
        PROFILE_TAB,
        META_TAB
    }

    [SerializeField] private KeyCode leftKeyCode;
    [SerializeField] private KeyCode rightKeyCode;

    [SerializeField] private Text leftText;
    [SerializeField] private Text rightText;

    [SerializeField] private TabType defaultTab = TabType.PLAY_TAB;

    public Tab[] tabs;
    public GameObject[] panels;

    private int currentTabIndex;

    void Start()
    {
        ConfigureTabs();
        ConfigureTexts();

        currentTabIndex = (int)defaultTab;
        tabs[currentTabIndex].Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(leftKeyCode))
        {
            if (currentTabIndex - 1 >= 0)
                tabs[currentTabIndex - 1].Select();
        }
        else if (Input.GetKeyDown(rightKeyCode))
        {
            if (currentTabIndex + 1 < tabs.Length)
                tabs[currentTabIndex + 1].Select();
        }
    }

    private void SelectCallback(int index)
    {
        if (index >= tabs.Length) return;
        if (index < 0) return;

        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == index) continue;
            tabs[i].Deselect();
            panels[i].SetActive(false);
        }

        panels[index].SetActive(true);

        currentTabIndex = index;
    }

    private void ConfigureTabs()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            TabType type = (TabType)i;
            tabs[i].callbacks.OnSelected.AddListener(() => {
                SelectCallback((int)type);
            });
        }
    }

    private void ConfigureTexts()
    {
        rightText.text = rightKeyCode.ToString();
        leftText.text = leftKeyCode.ToString();
    }
}
