using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using AssetPackage;
using UBlockly.UGUI;

public class LevelTestManager : MonoBehaviour
{
    [SerializeField] Category levelsCreatedCategory;
    [SerializeField] CameraFit cameraFit;
    [SerializeField] OrbitCamera cameraOrbit;
    [SerializeField] CameraZoom cameraZoom;

    [SerializeField] GameObject levelObjects;
    [SerializeField] GameObject levelCanvas;
    [SerializeField] GameObject levelButtons;
    [SerializeField] RectTransform levelViewport;

    [SerializeField] GameObject creatorObjects;
    [SerializeField] GameObject creatorCanvas;
    [SerializeField] RectTransform creatorViewPort;

    [SerializeField] BoardManager board;
    [SerializeField] BoardCreator boardCreator;
    [SerializeField] TextAsset activeBlocks;

    [SerializeField] GameObject debugPanel;
    [SerializeField] GameObject changeModeButton;
    [SerializeField] Sprite changeToEditModeSprite;
    [SerializeField] Sprite changeToPlayModeSprite;

    [SerializeField] GameObject loadBoardPanel;
    [SerializeField] GameObject saveBoardPanel;

    [SerializeField] StreamRoom streamRoom;

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
            streamRoom.FinishLevel();
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
            initialState = board.GetBoardState();
            cameraFit.SetViewPort(levelViewport);

            changeModeButton.GetComponent<Image>().sprite = changeToEditModeSprite;
            board.SetFocusPointOffset(new Vector3((board.GetColumns() - 2) / 2.0f + 0.5f, 0.0f, (board.GetRows() - 2) / 2.0f + 0.5f));
            cameraFit.FitBoard(board.GetRows(), board.GetColumns());
        }
        else
        {
            cameraOrbit.ResetInmediate();
            cameraZoom.ResetInmediate();
            cameraFit.SetViewPort(creatorViewPort);
            changeModeButton.GetComponent<Image>().sprite = changeToPlayModeSprite;
            boardCreator.FitBoard();
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
        streamRoom.Retry();
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
