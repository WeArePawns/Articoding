using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusManager : MonoBehaviour
{
    public GameObject blackPanel;
    public GameObject exitConfirmationPanel;
    public GameObject settingsMenu;



    void Start()
    {
        exitConfirmationPanel.SetActive(false);
        blackPanel.SetActive(false);
        settingsMenu.SetActive(false);
    }

    void Update()
    {
        
    }

    public void toggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void setActiveExitConfirmationPanel(bool active)
    {
        exitConfirmationPanel.SetActive(active);
        blackPanel.SetActive(active);
    }

    public void exitGame()
    {
        //GameManager.instance
        Application.Quit();
    }
}
