using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private GameObject levelParent;
    private Level levelObject;

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

    // TODO: Comprobar mensaje de final de partida
    /*private void recieveMessage(Message message)
    {

    }*/
}
