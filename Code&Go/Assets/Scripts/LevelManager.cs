using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UBlockly.UGUI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private GameObject levelParent;
    private Level levelObject;
    [Space]
    [SerializeField] private StatementManager  statementManager;

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

        if(levelObject == null)
        {
            Debug.LogError("Object instantiation failed");
        }

        levelObject.OnLevelStarted();

        // Maybe do more stuff
        statementManager.Load(currentLevel.statement);
        LoadInitialBlocks(currentLevel.initialState);
    }

    public void LoadLevel(LevelData level)
    {
        currentLevel = level;
        Initialize();
    }

    public void ResetLevel()
    {
        levelObject.ResetLevel();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}
