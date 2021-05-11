using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    [SerializeField] private List<Category> categories;
    [SerializeField] private bool allUnlocked;
    [SerializeField] private TextAsset activeBlocks;
    [SerializeField] private Category levelsCreatedCategory;

    private CategorySaveData[] categoriesData;
    private int hintsRemaining = 5;
    private int coins = 10;

    private CategorySaveData currentCategory = null;
    private int currentLevel = 0, lastCategoryUnlocked = 0;

    private LevelsCreatedSaveData levelsCreated;
    private List<string> levelsCreatedHash;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
    }

    private void Init()
    {
        categoriesData = new CategorySaveData[categories.Count];
        for (int i = 0; i < categoriesData.Length; i++)
        {
            CategorySaveData data = new CategorySaveData();
            data.lastLevelUnlocked = i <= lastCategoryUnlocked ? 0 : -1;
            data.totalStars = 0;
            data.levelsData = new LevelSaveData[categories[i].levels.Count];
            for (int j = 0; j < data.levelsData.Length; j++)
            {
                data.levelsData[j] = new LevelSaveData();
                data.levelsData[j].stars = 0;
            }
            categoriesData[i] = data;
        }

        currentCategory = categoriesData[0];
        currentLevel = 0;
        levelsCreated = new LevelsCreatedSaveData();
        levelsCreated.levelsCreated = new string[0];
        levelsCreatedHash = new List<string>();
        levelsCreatedCategory.levels.Clear();
    }

    //Setters
    //---------
    public void LevelCompleted(uint starsAchieved)
    {
        uint newStarsAchieved = (uint)Mathf.Clamp((float)starsAchieved - (float)currentCategory.levelsData[currentLevel].stars, 0.0f, 3.0f);
        currentCategory.levelsData[currentLevel].stars = currentCategory.levelsData[currentLevel].stars + newStarsAchieved;
        currentCategory.totalStars += newStarsAchieved;

        if (currentLevel >= currentCategory.lastLevelUnlocked)
        {
            if (currentLevel + 1 >= currentCategory.levelsData.Length && lastCategoryUnlocked < categories.Count)
            {
                lastCategoryUnlocked++;
                categoriesData[lastCategoryUnlocked].lastLevelUnlocked = 0;
            }
            currentCategory.lastLevelUnlocked = currentLevel + 1;
        }

    }

    public void LevelStarted(int categoryIndex, int level)
    {
        if (currentCategory == null || (categoryIndex >= 0 && categoryIndex < categoriesData.Length && categoriesData[categoryIndex] != currentCategory))
            currentCategory = categoriesData[categoryIndex];
        currentLevel = level;
    }

    public void LevelStarted(Category category, int level)
    {
        LevelStarted(categories.IndexOf(category), level);
    }

    public void AddHints(int amount)
    {
        hintsRemaining += amount;
        Mathf.Clamp(hintsRemaining, 0, 100);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Mathf.Clamp(coins, 0, 100);
    }

    //Getters
    //----------

    public bool IsLevelUnlocked(int categoryIndex, int level)
    {
        return allUnlocked || (categoryIndex < categoriesData.Length && level < categoriesData[categoryIndex].levelsData.Length &&
            level <= categoriesData[categoryIndex].lastLevelUnlocked);
    }

    public bool IsCategoryUnlocked(int categoryIndex)
    {
        return allUnlocked || (categoryIndex <= lastCategoryUnlocked);
    }

    public uint GetLevelStars(int categoryIndex, int level)
    {
        if (categoryIndex >= categoriesData.Length || categoryIndex < 0 || level >= categoriesData[categoryIndex].levelsData.Length || level < 0) return 0;
        return categoriesData[categoryIndex].levelsData[level].stars;
    }

    public uint GetLevelStars(Category category, int level)
    {
        return GetLevelStars(categories.IndexOf(category), level);
    }

    public uint GetCategoryProgress(int categoryIndex)
    {
        if (categoryIndex >= categoriesData.Length || categoryIndex < 0) return 0;
        return categoriesData[categoryIndex].totalStars;
    }

    public uint GetCategoryProgress(Category category)
    {
        return GetCategoryProgress(categories.IndexOf(category));
    }

    public int GetHintsRemaining()
    {
        return hintsRemaining;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void UserCreatedLevel(string board)
    {
        //si el nivel ya existe no se guarda
        if (levelsCreatedHash.Contains(Hash.ToHash(board, ""))) return;

        int index = levelsCreatedCategory.levels.Count + 1;
        string levelName = "created_level_" + index.ToString();
        string path =
#if UNITY_EDITOR
                   Application.dataPath;
#else
                   Application.persistentDataPath;
#endif
        string filePath = Path.Combine(path, "Boards/LevelsCreated/" + levelName + ".userLevel");
        FileStream file = new FileStream(filePath, FileMode.Create);
        file.Close();
        StreamWriter writer = new StreamWriter(filePath);
        writer.Write(board);
        writer.Close();

        AddLevelCreated(board, index);
        Array.Resize(ref levelsCreated.levelsCreated, levelsCreated.levelsCreated.Length + 1);
        levelsCreated.levelsCreated[levelsCreated.levelsCreated.GetUpperBound(0)] = levelName;
    }

    private void LoadLevelsCreated()
    {
        string path =
#if UNITY_EDITOR
                   Application.dataPath;
#else
                   Application.persistentDataPath;
#endif
        for (int i = 0; i < levelsCreated.levelsCreated.Length; i++)
        {
            string levelName = levelsCreated.levelsCreated[i];
            string filePath = Path.Combine(path, "Boards/LevelsCreated/" + levelName + ".userLevel");
            try
            {
                StreamReader reader = new StreamReader(filePath);
                string readerData = reader.ReadToEnd();
                reader.Close();
                AddLevelCreated(readerData, i + 1);
            }
            catch
            {                
                Debug.Log("El archivo " + filePath + " no existe");
            }
        }
    }

    private void AddLevelCreated(string board, int index)
    {
        LevelData data = ScriptableObject.CreateInstance<LevelData>();
        data.description = "Nivel creado por el usuario";
        data.activeBlocks = activeBlocks;
        data.levelName = "Nivel Creado " + index.ToString();
        data.auxLevelBoard = board;
        data.minimosPasos = 10;

        levelsCreatedCategory.levels.Add(data);
        levelsCreatedHash.Add(board);
    }

    //Save and Load
    //----------------
    public ProgressSaveData Save()
    {
        ProgressSaveData data = new ProgressSaveData();
        data.categoriesInfo = categoriesData;
        data.hintsRemaining = hintsRemaining;
        data.lastCategoryUnlocked = lastCategoryUnlocked;
        data.coins = coins;
        data.levelsCreatedData = levelsCreated;
        return data;
    }

    public void Load(ProgressSaveData data)
    {
        categoriesData = data.categoriesInfo;
        hintsRemaining = data.hintsRemaining;
        lastCategoryUnlocked = data.lastCategoryUnlocked;
        coins = data.coins;
        levelsCreated = data.levelsCreatedData;
        LoadLevelsCreated();
    }
}
