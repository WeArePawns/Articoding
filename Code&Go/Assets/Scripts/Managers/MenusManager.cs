using AssetPackage;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Simva;
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
        //TrackerAsset.Instance.Accessible.Accessed("menu")

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

        TrackerAsset.Instance.setVar("state", settingsMenu.activeSelf ? "opened" : "closed");
        TrackerAsset.Instance.GameObject.Interacted("settings_button");
    }

    public void SetActiveOptionsPanel(bool active)
    {
        optionsMenu.SetActive(active);
        blackPanel.SetActive(active);

        if (active)
            TrackerAsset.Instance.Accessible.Accessed("options_panel", AccessibleTracker.Accessible.Screen);
        else
            TrackerAsset.Instance.GameObject.Interacted("options_panel_close_button");
    }

    public void SetActiveExitConfirmationPanel(bool active)
    {
        exitConfirmationPanel.SetActive(active);
        blackPanel.SetActive(active);


        if (active)
            TrackerAsset.Instance.Accessible.Accessed("exit_game_panel", AccessibleTracker.Accessible.Screen);
        else
            TrackerAsset.Instance.GameObject.Interacted("exit_game_panel_close_button");
    }

    public void LoadCreditsScene()
    {
        TrackerAsset.Instance.Accessible.Accessed("credits", AccessibleTracker.Accessible.Screen);

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
        bool gameCompleted = ProgressManager.Instance.GetGameProgress() == 1f;
        //TrackerAsset.Instance.Completable.Completed("articoding", CompletableTracker.Completable.Game, gameCompleted, ProgressManager.Instance.GetTotalStars());

        SimvaExtension.Instance.Quit();
    }

    public void TraceEditor()
    {
        TrackerAsset.Instance.Accessible.Accessed("editor_levels", AccessibleTracker.Accessible.Screen);
    }
}
