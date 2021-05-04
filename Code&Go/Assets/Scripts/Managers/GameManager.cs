using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Category[] categories;
    [SerializeField] private bool loadSave = true;

    [SerializeField] private Category category;
    public int levelIndex;

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
        if (loadSave)
            SaveManager.Load();
    }

    public Category GetCurrentCategory()
    {
        return category;
    }

    public bool InCreatedLevel()
    {
        return category = categories[categories.Length - 1];
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
        ProgressManager.Instance.LevelStarted(category, levelIndex);
        if (loadSave)
            SaveManager.Save();
        this.category = category;
        this.levelIndex = levelIndex;
        LoadScene("LevelScene");
    }

    public void LoadLevelCreator()
    {
        LoadScene("BoardCreation");
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void OnDestroy()
    {
        if (Instance == this && loadSave)
            SaveManager.Save();
    }
}
