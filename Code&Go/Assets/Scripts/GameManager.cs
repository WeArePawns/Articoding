using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Category[] categories;
    [SerializeField] private bool loadSave = true;

    private Category category;
    public int levelIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SaveManager.Init();
            if (loadSave)
                SaveManager.Load();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public Category GetCurrentCategory()
    {
        return category;
    }

    public int GetCurrentLevelIndex()
    {
        return levelIndex;
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
