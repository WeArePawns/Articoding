using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using AssetPackage;
using UBlockly.UGUI;
using UnityEditor;


public class LevelTestManager : MonoBehaviour
{
    [SerializeField] Category levelsCreatedCategory;

    [SerializeField] GameObject levelObjects;
    [SerializeField] GameObject levelCanvas;
    [SerializeField] GameObject levelButtons;
    [SerializeField] CameraFit cameraFit;

    [SerializeField] GameObject creatorObjects;
    [SerializeField] GameObject creatorCanvas;

    [SerializeField] BoardManager board;
    [SerializeField] BoardCreator boardCreator;
    [SerializeField] TextAsset activeBlocks;    

    [SerializeField] GameObject debugPanel;
    [SerializeField] GameObject changeModeButton;
    [SerializeField] Sprite changeToEditModeSprite;
    [SerializeField] Sprite changeToPlayModeSprite;

    [SerializeField] GameObject loadBoardPanel;
    [SerializeField] GameObject saveBoardPanel;

    public GameObject endPanel;
    public GameObject blackRect;
    public GameObject endPanelMinimized;

    public GameObject gameOverPanel;
    public GameObject gameOverMinimized;

    private bool inCreator = false;
    private BoardState initialState;
    private bool completed = false;

    private void Start()
    {
        Invoke("ChangeMode", 0.01f);
        ActivateLevelBlocks(activeBlocks, false);
#if UNITY_EDITOR
        loadBoardPanel.SetActive(true);
        saveBoardPanel.SetActive(true);
#endif
    }

    private void Update()
    {
        if (inCreator)
        {
            bool enabled = board.GetNEmitters() == board.GetNReceivers() && board.GetNEmitters() > 0 && !board.AllReceiving();
            changeModeButton.GetComponent<Button>().enabled = enabled;
            changeModeButton.GetComponent<Image>().color = enabled ? Color.white : Color.red;
        }
        else if (board.BoardCompleted() && !completed)
        {
            completed = true;
            endPanel.SetActive(true);
            blackRect.SetActive(true);
            ProgressManager.Instance.UserCreatedLevel(initialState.ToJson());
        }
    }

    public void ChangeMode()
    {
        inCreator = !inCreator;

        levelObjects.SetActive(!inCreator);
        levelCanvas.SetActive(!inCreator);
        levelButtons.SetActive(!inCreator);

        creatorObjects.SetActive(inCreator);
        creatorCanvas.SetActive(inCreator);

        board.SetModifiable(inCreator);

        if (!inCreator)
        {
            completed = false;
            cameraFit.FitBoard(board.GetRows(), board.GetColumns());
            initialState = board.GetBoardState();

            changeModeButton.GetComponent<Image>().sprite = changeToEditModeSprite;
        }
        else
        {
            board.transform.position = Vector3.zero;
            board.transform.localScale = Vector3.one;
            boardCreator.FitBoard();

            changeModeButton.GetComponent<Image>().sprite = changeToPlayModeSprite;
        }
    }

    public void LoadMainMenu()
    {
        //TrackerAsset.Instance.setVar("steps", board.GetCurrentSteps());
        TrackerAsset.Instance.GameObject.Used("main_menu_return");
        GameManager.Instance.LoadScene("MenuScene");
    }

    public void ResetLevel()
    {
        board.Reset();
        board.GenerateBoardElements(initialState);
        debugPanel.SetActive(true);
        cameraFit.FitBoard(board.GetRows(), board.GetColumns());
    }

    public void RetryLevel()
    {
        ResetLevel();
        gameOverPanel.SetActive(false);
        blackRect.SetActive(false);
        gameOverMinimized.SetActive(false);
        completed = false;
    }

    public void MinimizeEndPanel()
    {
        endPanelMinimized.SetActive(true);
        gameOverPanel.SetActive(false);
        endPanel.SetActive(false);
        blackRect.SetActive(false);
        debugPanel.SetActive(false);
    }

    public void MinimizeGameOverPanel()
    {
        gameOverMinimized.SetActive(true);
        gameOverPanel.SetActive(false);
        //endPanel.SetActive(false);
        blackRect.SetActive(false);
        debugPanel.SetActive(false);
    }

    public void ActivateLevelBlocks(TextAsset textAsset, bool allActive)
    {
        if (textAsset == null) return;

        StartCoroutine(AsyncActivateLevelBlocks(textAsset, allActive));
    }

    IEnumerator AsyncActivateLevelBlocks(TextAsset textAsset, bool allActive)
    {
        if (allActive) BlocklyUI.WorkspaceView.Toolbox.SetActiveAllBlocks();
        else if (textAsset != null)
        {
            ActiveBlocks blocks = ActiveBlocks.FromJson(textAsset.text);
            BlocklyUI.WorkspaceView.Toolbox.SetActiveBlocks(blocks.AsMap());
        }

        yield return null;
    }
}
