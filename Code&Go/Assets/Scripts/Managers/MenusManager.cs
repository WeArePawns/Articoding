using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenusManager : MonoBehaviour
{
    public GameObject blackPanel;
    public GameObject exitConfirmationPanel;
    public GameObject settingsMenu;
    public RectTransform settingsButtonTransform;
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
                Rect settingsRect = RectUtils.RectTransformToScreenSpace(settingsMenu.GetComponent<RectTransform>());
                Rect settingsButtonRect = RectUtils.RectTransformToScreenSpace(settingsButtonTransform);
                // Out of settings rect AND settings button rect
                if ((mousePos.x < settingsRect.x || mousePos.x > settingsRect.x + settingsRect.width || mousePos.y < settingsRect.y || mousePos.y > settingsRect.y + settingsRect.height) &&
                    (mousePos.x < settingsButtonRect.x || mousePos.x > settingsButtonRect.x + settingsButtonRect.width || mousePos.y < settingsButtonRect.y || mousePos.y > settingsButtonRect.y + settingsButtonRect.height))
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

    public void LoadCreditsScene()
    {
        if (LoadManager.Instance == null)
        {
            SceneManager.LoadScene("EndScene");
            return;
        }

        LoadManager.Instance.LoadScene("EndScene");
    }

    public void ExitGame()
    {
        //GameManager.instance.Quit(); //TODO: GameManager

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); ;
#endif

        Application.Quit();
    }
}
