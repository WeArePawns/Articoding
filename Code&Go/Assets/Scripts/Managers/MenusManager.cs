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

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void SetActiveExitConfirmationPanel(bool active)
    {
        exitConfirmationPanel.SetActive(active);
        blackPanel.SetActive(active);
    }

    public void ExitGame()
    {
        //GameManager.instance.Quit(); //TODO: GameManager
        Application.Quit();
    }
}
