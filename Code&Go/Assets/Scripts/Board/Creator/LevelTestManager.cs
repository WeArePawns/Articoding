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
    }

    private void Update()
    {
        if (inCreator)
        {
            bool enabled = board.GetNEmitters() == board.GetNReceivers() && board.GetNEmitters() > 0 && !board.BoardCompleted();
            changeModeButton.GetComponent<Button>().enabled = enabled;
            changeModeButton.GetComponent<Image>().color = enabled ? Color.white : Color.red;
        }
        else
        {
            if (board.BoardCompleted() && !completed)
            {
                completed = true;
                int index = levelsCreatedCategory.levels.Count + 1;
                string levelName = "created_level_" + index.ToString();

                LevelData data = ScriptableObject.CreateInstance<LevelData>();
                data.description = "Nivel creado por el usuario";
                data.activeBlocks = activeBlocks;
                data.levelName = "Nivel Creado " + index.ToString();
                data.levelBoard = null;

                levelsCreatedCategory.levels.Add(data);
            }
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

        board.transform.position = Vector3.zero;
        board.transform.localScale = Vector3.one;
        if (!inCreator)
        {
            completed = false;
            cameraFit.FitBoard(board.GetRows(), board.GetColumns());
            initialState = board.GetBoardStateWithoutHoles();
        }
        else boardCreator.FitBoard();
    }

    public void LoadLevelToTest()
    {

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
        BoardState state = BoardState.FromJson(initialState.ToJson());
        board.GenerateBoardElements(state);
        debugPanel.SetActive(true);
        cameraFit.FitBoard(board.GetRows(), board.GetColumns());
    }

    public void RetryLevel()
    {
        ResetLevel();
        gameOverPanel.SetActive(false);
        blackRect.SetActive(false);
        gameOverMinimized.SetActive(false);
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
