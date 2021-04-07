﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // TODO: quitar
    public Category exampleCategory;

    private Category category;
    public int levelIndex;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        this.category = category;
        this.levelIndex = levelIndex;
        LoadScene("LevelScene");
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
