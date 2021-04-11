using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    [SerializeField]
    private List<Category> categories;

    private CategorySaveData[] categoriesData;
    private int hintsRemaining = 5;
    private int coins = 10;

    private CategorySaveData currentCategory = null;
    private int currentLevel = 0;

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
            data.lastLevelUnlocked = 0;
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
        currentCategory.levelsData[currentLevel].stars = starsAchieved;
        currentCategory.totalStars += GetNStars(starsAchieved);

        if (currentLevel >= currentCategory.lastLevelUnlocked)
            currentCategory.lastLevelUnlocked = currentLevel+1;
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
        return (categoryIndex < categoriesData.Length && level < categoriesData[categoryIndex].levelsData.Length &&
            level <= categoriesData[categoryIndex].lastLevelUnlocked);
    }

    public uint GetLevelStars(int categoryIndex, int level)
    {
        if (categoryIndex >= categoriesData.Length || level >= categoriesData[categoryIndex].levelsData.Length) return 0;
        return categoriesData[categoryIndex].levelsData[level].stars;
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
        data.coins = coins;
        return data;
    }

    public void Load(ProgressSaveData data)
    {
        categoriesData = data.categoriesInfo;
        hintsRemaining = data.hintsRemaining;
        coins = data.coins;
    }

    //Returns how many bits are active
    private uint GetNStars(uint stars)
    {      
        uint c; // c accumulates the total bits set in v
        for (c = 0; stars!=0; c++)
        {
            stars &= stars - 1; // clear the least significant bit set
        }
        return c;
    }
}
