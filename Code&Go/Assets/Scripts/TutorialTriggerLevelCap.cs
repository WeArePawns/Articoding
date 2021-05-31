using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialTriggerLevelCap : MonoBehaviour
{

    public TutorialTrigger tt;
    public LevelData level;

    void Awake()
    {
        GameManager gm = GameManager.Instance;
        if (gm.GetCurrentCategory().levels[gm.GetCurrentLevelIndex()] == level)
            tt.enabled = true;
        else
            tt.enabled = false;

        if (ProgressManager.Instance.IsAllUnlockedModeOn())
            tt.enabled = true;

    }
}
