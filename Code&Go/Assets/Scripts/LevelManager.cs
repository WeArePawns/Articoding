using System.Collections;
using System.Collections.Generic;
using System.Xml;
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
    }

    public void LoadLevel(LevelData level)
    {
        currentLevel = level;
        Initialize();
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
