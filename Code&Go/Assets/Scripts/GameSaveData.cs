using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSaveData
{
    public uint stars;// 3bits 1 por estrella
}

[System.Serializable]
public class CategorySaveData
{
    public LevelSaveData[] levelsData;
    public int lastLevelUnlocked;
    public uint totalStars;
    public bool completableInitialized;
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
