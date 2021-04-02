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
    [SerializeField] private Vector2 boardInitOffset;
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
        boardInitOffset = new Vector2(Mathf.Clamp(boardInitOffset.x, 0.0f, 1.0f), Mathf.Clamp(boardInitOffset.y, 0.0f, 1.0f));
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
        boardManager.LoadBoard(currentLevel.levelBoard);
        FitBoard();
    }

    private void FitBoard()
    {
        if (mainCamera != null)
        {
            float height = mainCamera.orthographicSize * 2, width = height * mainCamera.aspect;
            float xPos = Mathf.Lerp(-width / 2.0f, width / 2.0f, boardInitOffset.x);
            height *= (1.0f - boardInitOffset.y);
            width *= (1.0f - boardInitOffset.x);
            float boardHeight = (float)boardManager.GetRows(), boardWidth = (float)boardManager.GetColumns();
            float xRatio = width / boardWidth, yRatio = height / boardHeight;
            float ratio = Mathf.Min(xRatio, yRatio);
            float offsetX = (width - boardWidth * ratio) / 2.0f + 0.5f * ratio, offsetY = (height - boardHeight * ratio) / 2.0f + 0.5f * ratio;

            //Fit the board on the screen and resize it
            boardManager.transform.position = new Vector3(xPos + offsetX, -height / 2.0f + offsetY, 0);
            boardManager.transform.localScale = new Vector3(ratio, ratio, 1.0f);
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
        boardManager.LoadBoard(currentLevel.levelBoard);
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
