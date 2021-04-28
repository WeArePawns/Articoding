using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    [SerializeField] private List<Category> categories;
    [SerializeField] private bool allUnlocked;

    private CategorySaveData[] categoriesData;
    private int hintsRemaining = 5;
    private int coins = 10;

    private CategorySaveData currentCategory = null;
    private int currentLevel = 0, lastCategoryUnlocked = 0;

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
            data.levelsData = new LevelSaveData[categories[i].levels.Length];
            for (int j = 0; j < data.levelsData.Length; j++)
            {
                data.levelsData[j] = new LevelSaveData();
                data.levelsData[j].stars = 0;
            }
            categoriesData[i] = data;
        }

        currentCategory = categoriesData[0];
        currentLevel = 0;
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
        if (currentCategory == null || categoriesData[categoryIndex] != currentCategory)
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
        if (categoryIndex >= categoriesData.Length || level >= categoriesData[categoryIndex].levelsData.Length) return 0;
        return categoriesData[categoryIndex].levelsData[level].stars;
    }

    public uint GetLevelStars(Category category, int level)
    {
        return GetLevelStars(categories.IndexOf(category), level);
    }

    public uint GetCategoryProgress(int categoryIndex)
    {
        if (categoryIndex >= categoriesData.Length) return 0;
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

    //Save and Load
    //----------------
    public ProgressSaveData Save()
    {
        ProgressSaveData data = new ProgressSaveData();
        data.categoriesInfo = categoriesData;
        data.hintsRemaining = hintsRemaining;
        data.lastCategoryUnlocked = lastCategoryUnlocked;
        data.coins = coins;
        return data;
    }

    public void Load(ProgressSaveData data)
    {
        categoriesData = data.categoriesInfo;
        hintsRemaining = data.hintsRemaining;
        lastCategoryUnlocked = data.lastCategoryUnlocked;
        coins = data.coins;
    }
}
