using System.Collections;
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
    }

    private void Update()
    {
        if (boardManager == null)
            return;

        if (boardManager.GetCurrentPasos() > minimosPasos)
        {
            starsController.deactivateMinimoStar();
        }

        if (boardManager.BoardCompleted() && !endPanel.activeSelf && !endPanelMinimized.activeSelf)
        {
            endPanel.SetActive(true);
            blackRect.SetActive(true);
        }
    }

    private void Initialize()
    {
        if (currentLevel == null)
        {
            Debug.LogError("Cannot initialize Level. CurrentLevel is null");
            return;
        }

        // Maybe do more stuff
        statementManager.Load(currentLevel.statement);
        ActivateLevelBlocks(currentLevel.activeBlocks, currentLevel.allActive);
        LoadInitialBlocks(currentLevel.initialState);
        BoardState state = BoardState.FromJson(currentLevel.levelBoard.text);
        boardManager.LoadBoard(state, buildLimits);
        FitBoard();
    }

    private void FitBoard()
    {
        if (mainCamera != null)
        {
            float height = mainCamera.orthographicSize * 2, width = height * mainCamera.aspect;
            float xPos = Mathf.Lerp(-width / 2.0f, width / 2.0f, boardInitOffsetLeftDown.x);
            float yPos = Mathf.Lerp(-height / 2.0f, height / 2.0f, boardInitOffsetLeftDown.y);
            height *= (1.0f - (boardInitOffsetLeftDown.y + boardInitOffsetRightUp.y));
            width *= (1.0f - (boardInitOffsetLeftDown.x + boardInitOffsetRightUp.x));

            int limits = (buildLimits) ? 0 : 0;//Ver si queremos que se tenga en cuenta para el fit
            float boardHeight = (float)boardManager.GetRows() + limits, boardWidth = (float)boardManager.GetColumns() + limits;
            float xRatio = width / boardWidth, yRatio = height / boardHeight;
            float ratio = Mathf.Min(xRatio, yRatio);
            float offsetX = (-boardWidth * ratio) / 2.0f + (limits / 2.0f + 0.5f) * ratio, offsetY = (-boardHeight * ratio) / 2.0f + (limits / 2.0f + 0.5f) * ratio;

            //Fit the board on the screen and resize it
            boardManager.transform.position = new Vector3(xPos + width / 2.0f + offsetX, 0, yPos + height / 2.0f + offsetY);
            boardManager.transform.localScale = new Vector3(ratio, ratio, ratio);
        }
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
        ProgressManager.Instance.LevelCompleted(0x111);
        int levelSize = currentCategory.levels.Length;
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

        starsController.deactivatePrimeraEjecucionStar();
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

    //TODO:Hacer un reset board en vez de volver a cargarla... o no
    public void ResetLevel()
    {
        boardManager.DeleteBoardElements();
        BoardState state = BoardState.FromJson(currentLevel.levelBoard.text);
        boardManager.GenerateBoardElements(state);
        debugPanel.SetActive(true);
    }

    public void ReloadLevel()
    {
        LoadLevel(currentLevel);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
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
