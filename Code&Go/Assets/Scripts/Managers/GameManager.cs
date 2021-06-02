using AssetPackage;
using Simva.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Category[] categories;
    [SerializeField] private bool loadSave = true;

    [SerializeField] private Category category;
    public int levelIndex;
    private bool gameLoaded = false;

    private Dictionary<UBlockly.Block, string> blockIDs;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);            
            SaveManager.Init();
        }
        else
            DestroyImmediate(gameObject);
    }

    private void Start()
    {
        LoadGame();
        TrackerAsset.Instance.setVar("language", LocalizationSettings.SelectedLocale.Identifier.Code);
        TrackerAsset.Instance.setVar("resolution", Screen.currentResolution.ToString());
        TrackerAsset.Instance.setVar("fullscreen", Screen.fullScreen);

        TrackerAsset.Instance.Completable.Initialized("articoding", CompletableTracker.Completable.Game);
        TrackerAsset.Instance.Completable.Progressed("articoding", CompletableTracker.Completable.Game, ProgressManager.Instance.GetGameProgress());
    }

    public void LoadGame()
    {
        if (loadSave && !gameLoaded)
        {
            SaveManager.Load();
            gameLoaded = true;
        }
    }

    public bool IsGameLoaded()
    {
        return gameLoaded;
    }

    public Category GetCurrentCategory()
    {
        return category;
    }

    public bool InCreatedLevel()
    {
        return category == categories[categories.Length - 1];
    }

    public int GetCurrentLevelIndex()
    {
        return levelIndex;
    }

    public void SetCurrentLevel(int levelIndex)
    {
        this.levelIndex = levelIndex;
    }

    public void SetCurrentCategory(Category category)
    {
        this.category = category;
    }

    // Esto habra que moverlo al MenuManager o algo asi
    public void LoadLevel(Category category, int levelIndex)
    {
        blockIDs = new Dictionary<UBlockly.Block, string>();
        this.category = category;
        this.levelIndex = levelIndex;

        ProgressManager.Instance.LevelStarted(category, levelIndex);
        if (loadSave)
            SaveManager.Save();

        if (LoadManager.Instance == null)
        {
            SceneManager.LoadScene("LevelScene");
            return;
        }

        LoadManager.Instance.LoadScene("LevelScene");
    }

    public void LoadLevelCreator()
    {
        blockIDs = new Dictionary<UBlockly.Block, string>();
        levelIndex = -1;
        if (LoadManager.Instance == null)
        {
            SceneManager.LoadScene("BoardCreation");
            return;
        }
        LoadManager.Instance.LoadScene("BoardCreation");
    }

    public void LoadScene(string name)
    {
        if (LoadManager.Instance == null)
        {
            SceneManager.LoadScene(name);
            return;
        }
        LoadManager.Instance.LoadScene(name);
    }

    public void OnDestroy()
    {
        if (Instance == this && loadSave)
            SaveManager.Save();
    }

    public void OnApplicationQuit()
    {
        if (loadSave)
            SaveManager.Save();
    }

    public string GetCurrentLevelName()
    {
        if (this.levelIndex == -1)
            return "editor_level";

        Category category = GetCurrentCategory();
        int levelIndex = GetCurrentLevelIndex();
        string levelName = category.levels[levelIndex].levelName;

        return levelName;
    }

    public Category[] GetCategories()
    {
        return categories;
    }

    public string GetBlockId(UBlockly.Block block)
    {
        while (!blockIDs.ContainsKey(block))
        {
            var blockId = block.Type + "_" + SimvaPlugin.SimvaApi<IStudentsApi>.GenerateRandomBase58Key(4);
            if (!blockIDs.ContainsValue(blockId))
                blockIDs.Add(block, blockId);
        }
        return blockIDs[block];
    }
    
    public string ChangeCodeIDs(string code)
    {
        foreach(var kv in blockIDs)       
            code = code.Replace(kv.Key.ID, kv.Value);
        return code;
    }
}
