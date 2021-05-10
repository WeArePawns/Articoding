using AssetPackage;
using System.Collections;
using System.Collections.Generic;
using UBlockly.UGUI;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Category currentCategory;
    [SerializeField] private LevelData currentLevel;
    private int currentLevelIndex = 0;

    //Values between 0 and 1 that indicate the limits of the board
    [SerializeField] private Vector2 boardInitOffsetLeftDown;
    [SerializeField] private Vector2 boardInitOffsetRightUp;
    [SerializeField] private bool buildLimits = true;

    [Space]
    [SerializeField] private StatementManager statementManager;

    [SerializeField] private BoardManager boardManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFit cameraFit;

    [SerializeField] private Category defaultCategory;
    [SerializeField] private int defaultLevelIndex;

    public GameObject endPanel;
    public GameObject blackRect;

    public GameObject endPanelMinimized;
    public GameObject debugPanel;

    public GameObject gameOverPanel;
    public GameObject gameOverMinimized;

    public StarsController starsController;

    private int minimosPasos = 0;

    private void Awake()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager != null)
        {
            currentCategory = gameManager.GetCurrentCategory();
            currentLevelIndex = gameManager.GetCurrentLevelIndex();
            currentLevel = currentCategory.levels[currentLevelIndex];
            minimosPasos = currentLevel.minimosPasos;
        }
        else
        {
            currentCategory = defaultCategory;
            currentLevelIndex = defaultLevelIndex;
            currentLevel = currentCategory.levels[currentLevelIndex];
            minimosPasos = currentLevel.minimosPasos;
        }

        //Clamp values between 0 and 1
        boardInitOffsetRightUp = new Vector2(Mathf.Clamp(boardInitOffsetRightUp.x, 0.0f, 1.0f), Mathf.Clamp(boardInitOffsetRightUp.y, 0.0f, 1.0f));
        boardInitOffsetLeftDown = new Vector2(Mathf.Clamp(boardInitOffsetLeftDown.x, 0.0f, 1.0f), Mathf.Clamp(boardInitOffsetLeftDown.y, 0.0f, 1.0f));
        if (boardInitOffsetLeftDown.x + boardInitOffsetRightUp.x >= 1.0f)
            boardInitOffsetLeftDown.x = boardInitOffsetRightUp.x = 0;
        if (boardInitOffsetLeftDown.y + boardInitOffsetRightUp.y >= 1.0f)
            boardInitOffsetLeftDown.y = boardInitOffsetRightUp.y = 0;

        endPanel.SetActive(false);
        //blackRect.SetActive(false);
    }

    private void Start()
    {
        Initialize();

        TrackerAsset.Instance.setVar("category_id", currentCategory.name_id);
        TrackerAsset.Instance.setVar("level_id", currentLevelIndex);
        TrackerAsset.Instance.GameObject.Used("level_start");
    }

    private void Update()
    {
        if (boardManager == null)
            return;

        if (boardManager.GetCurrentSteps() > minimosPasos)
            starsController.DeactivateMinimumStepsStar();


        if (boardManager.BoardCompleted() && !endPanel.activeSelf && !endPanelMinimized.activeSelf)
        {
            TrackerAsset.Instance.setVar("category_id", currentCategory.name_id);
            TrackerAsset.Instance.setVar("level_id", currentLevelIndex);
            TrackerAsset.Instance.setVar("steps", boardManager.GetCurrentSteps());
            TrackerAsset.Instance.setVar("first_execution", starsController.IsFirstRunStarActive());
            TrackerAsset.Instance.setVar("minimum_steps", starsController.IsMinimumStepsStarActive());
            TrackerAsset.Instance.setVar("no_hints", starsController.IsNoHintsStarActive());
            TrackerAsset.Instance.GameObject.Used("level_end");

            endPanel.SetActive(true);
            blackRect.SetActive(true);
            if (!GameManager.Instance.InCreatedLevel())
                ProgressManager.Instance.LevelCompleted(starsController.GetStars());
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.N))
            LoadNextLevel();
#endif
    }

    private void Initialize()
    {
        if (currentLevel == null)
        {
            Debug.LogError("Cannot initialize Level. CurrentLevel is null");
            return;
        }

        // Maybe do more stuff
        ActivateLevelBlocks(currentLevel.activeBlocks, currentLevel.allActive);
        LoadInitialBlocks(currentLevel.initialState);

        string boardJson = currentLevel.levelBoard != null ? currentLevel.levelBoard.text : currentLevel.auxLevelBoard;
        BoardState state = BoardState.FromJson(boardJson);
        boardManager.LoadBoard(state, buildLimits);
        cameraFit.FitBoard(boardManager.GetRows(), boardManager.GetColumns());
    }

    public void LoadLevel(Category category, int levelIndex)
    {
        currentCategory = category;
        currentLevelIndex = levelIndex;
        LoadLevel(category.levels[levelIndex]);
    }
    private void LoadLevel(LevelData level)
    {
        currentLevel = level;
        Initialize();
    }

    // It is called when the current level is completed
    public void LoadNextLevel()
    {
        int levelSize = currentCategory.levels.Count;
        if (++currentLevelIndex < levelSize)
            GameManager.Instance.LoadLevel(currentCategory, currentLevelIndex);
        else
            LoadMainMenu(); // Por ejemplo
    }

    public void RetryLevel()
    {
        ResetLevel();
        gameOverPanel.SetActive(false);
        blackRect.SetActive(false);
        gameOverMinimized.SetActive(false);

        starsController.DeactivateFirstRunStar();
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

    public void ResetLevel()
    {
        boardManager.Reset();
        string boardJson = currentLevel.levelBoard != null ? currentLevel.levelBoard.text : currentLevel.auxLevelBoard;
        BoardState state = BoardState.FromJson(boardJson);
        boardManager.GenerateBoardElements(state);
        debugPanel.SetActive(true);
        cameraFit.FitBoard(boardManager.GetRows(), boardManager.GetColumns());
    }

    public void ReloadLevel()
    {
        LoadLevel(currentLevel);
    }

    public void LoadMainMenu()
    {
        TrackerAsset.Instance.setVar("steps", boardManager.GetCurrentSteps());
        TrackerAsset.Instance.GameObject.Used("main_menu_return");

        GameManager.Instance.LoadScene("MenuScene");
    }

    // Copiado y modificado (TODO: cambiar de lugar si eso)
    public void LoadInitialBlocks(TextAsset textAsset)
    {
        if (textAsset == null) return;

        StartCoroutine(AsyncLoadInitialBlocks(textAsset));
    }

    IEnumerator AsyncLoadInitialBlocks(TextAsset textAsset)
    {
        BlocklyUI.WorkspaceView.CleanViews();

        var dom = UBlockly.Xml.TextToDom(textAsset.text);

        UBlockly.Xml.DomToWorkspace(dom, BlocklyUI.WorkspaceView.Workspace);
        BlocklyUI.WorkspaceView.BuildViews();

        yield return null;
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
