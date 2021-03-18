using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Level : MonoBehaviour
{
    public abstract bool IsCompleted();
    public abstract void OnLevelStarted();
    public abstract void OnLevelCompleted();
    public abstract void ResetLevel();
}
