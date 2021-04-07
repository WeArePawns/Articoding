using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UBlockly.UGUI;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Awake()
    {
        GameManager gameManager = GameManager.Instance;

        if (gameManager != null)
        {
            currentCategory = gameManager.GetCurrentCategory();
            if (currentCategory == null)
                currentCategory = gameManager.exampleCategory;
            currentLevelIndex = gameManager.GetCurrentLevelIndex();
            currentLevel = currentCategory.levels[currentLevelIndex];
        }

        //Clamp values between 0 and 1
        boardInitOffsetRightUp = new Vector2(Mathf.Clamp(boardInitOffsetRightUp.x, 0.0f, 1.0f), Mathf.Clamp(boardInitOffsetRightUp.y, 0.0f, 1.0f));
        boardInitOffsetLeftDown = new Vector2(Mathf.Clamp(boardInitOffsetLeftDown.x, 0.0f, 1.0f), Mathf.Clamp(boardInitOffsetLeftDown.y, 0.0f, 1.0f));
        if (boardInitOffsetLeftDown.x + boardInitOffsetRightUp.x >= 1.0f)
            boardInitOffsetLeftDown.x = boardInitOffsetRightUp.x = 0;
        if (boardInitOffsetLeftDown.y + boardInitOffsetRightUp.y >= 1.0f)
            boardInitOffsetLeftDown.y = boardInitOffsetRightUp.y = 0;

    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (boardManager == null)
            return;

        if (boardManager.BoardCompleted())
        {
            LoadNextLevel();
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
        boardManager.LoadBoard(currentLevel.levelBoard, buildLimits);
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

            int limits = (buildLimits) ? 2 : 0;
            float boardHeight = (float)boardManager.GetRows() + limits, boardWidth = (float)boardManager.GetColumns() + limits;
            float xRatio = width / boardWidth, yRatio = height / boardHeight;
            float ratio = Mathf.Min(xRatio, yRatio);
            float offsetX = (-boardWidth * ratio) / 2.0f + (limits / 2.0f + 0.5f) * ratio, offsetY = (-boardHeight * ratio) / 2.0f + (limits / 2.0f + 0.5f) * ratio;

            //Fit the board on the screen and resize it
            boardManager.transform.position = new Vector3(xPos + width / 2.0f + offsetX, yPos + height / 2.0f + offsetY, 0);
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
    private void LoadNextLevel()
    {
        int levelSize = currentCategory.levels.Length;
        if (++currentLevelIndex < levelSize)
            GameManager.Instance.LoadLevel(currentCategory, currentLevelIndex);
        else
            SceneManager.LoadScene("MenuScene"); // Por ejemplo
    }

    //TODO:Hacer un reset board en vez de volver a cargarla... o no
    public void ResetLevel()
    {
        boardManager.transform.localScale = Vector3.one;
        boardManager.transform.localPosition = Vector3.zero;
        boardManager.LoadBoard(currentLevel.levelBoard, buildLimits);
        FitBoard();
    }

    public void ReloadLevel()
    {
        LoadLevel(currentLevel);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
