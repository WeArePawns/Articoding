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

    [SerializeField] private GameObject levelParent;
    private Level levelObject;
    [Space]
    [SerializeField] private StatementManager statementManager;

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
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (levelObject == null)
            return;

        if (levelObject.IsCompleted())
        {
            levelObject.OnLevelCompleted();
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

        levelObject = Instantiate(currentLevel.levelPrefab, levelParent.transform);

        if (levelObject == null)
        {
            Debug.LogError("Object instantiation failed");
        }

        levelObject.OnLevelStarted();

        // Maybe do more stuff
        statementManager.Load(currentLevel.statement);
        LoadInitialBlocks(currentLevel.initialState);
        ActivateLevelBlocks(currentLevel.activeBlocks, currentLevel.allActive);
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

    public void ResetLevel()
    {
        levelObject.ResetLevel();
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
        if (allActive) BlocklyUI.WorkspaceView.Toolbox.SetActiveAllBlocks();
        else if (textAsset != null)
        {
            ActiveBlocks blocks = ActiveBlocks.FromJson(textAsset.text);
            BlocklyUI.WorkspaceView.Toolbox.SetActiveBlocks(blocks.AsMap());
        }
    }
}
