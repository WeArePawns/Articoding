using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public int stars = -1;// -1 if the level is not completed
}

[System.Serializable]
public class CategorySaveData
{
    public LevelSaveData[] levelsData;
    public int lastLevelUnlocked;
    public uint totalStars;
    public bool completableInitialized;

    public int GetLevelsCompleted()
    {
        int levelsCompleted = 0;
        foreach (LevelSaveData levelData in levelsData)
            levelsCompleted += levelData.stars >= 0 ? 1 : 0;
        return levelsCompleted;
    }
}

[System.Serializable]
public class LevelsCreatedSaveData
{
    public string[] levelsCreated;
}

[System.Serializable]
public class ProgressSaveData
{
    public LevelsCreatedSaveData levelsCreatedData;
    public CategorySaveData[] categoriesInfo;
    public int hintsRemaining;
    public int lastCategoryUnlocked;
    public int coins;
}

[System.Serializable]
public class TutorialSaveData
{
    public string[] tutorials;
}


[System.Serializable]
public class GameSaveData
{
    public ProgressSaveData progressData;
    public TutorialSaveData tutorialInfo;
}

[System.Serializable]
public class SaveData
{
    public GameSaveData gameData;
    public string hash;
}
