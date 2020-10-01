using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGroup : MonoBehaviour
{
    /* All panels */
    public GameObject[] panels;

    private int currentPanel;


    private void Start()
    {
        SetPanelIndex(0);
    }

    private void ShowCurrentPanel()
    {
        for(int i = 0; i < panels.Length; i++)
        {
            bool shown = (i == currentPanel);
            panels[i].SetActive(shown);
        }
    }

    public void SetPanelIndex(int index)
    {
        currentPanel = index;
        ShowCurrentPanel();
    }

}
