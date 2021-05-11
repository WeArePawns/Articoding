using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusManager : MonoBehaviour
{
    public GameObject blackPanel;
    public GameObject exitConfirmationPanel;
    public GameObject settingsMenu;
    public GameObject optionsMenu;

    void Start()
    {
        exitConfirmationPanel.SetActive(false);
        blackPanel.SetActive(false);
        settingsMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    private void Update()
    {
        if(settingsMenu.activeSelf && !blackPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                Rect rect = RectUtils.RectTransformToScreenSpace(settingsMenu.GetComponent<RectTransform>());
                if (mousePos.x < rect.x || mousePos.x > rect.x + rect.width || mousePos.y < rect.y || mousePos.y > rect.y + rect.height)
                    ToggleSettingsMenu();
            }
        }
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void SetActiveOptionsPanel(bool active)
    {
        optionsMenu.SetActive(active);
        blackPanel.SetActive(active);
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
