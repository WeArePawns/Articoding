using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGroup : MonoBehaviour
{
    /*
     * Esta es la clase que se modificará para cambiar
     * la forma en la que se cambian los paneles al 
     * deslizar con gestos.
     */


    /* All panels */
    public GameObject[] panels;

    private int currentPanel;


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
