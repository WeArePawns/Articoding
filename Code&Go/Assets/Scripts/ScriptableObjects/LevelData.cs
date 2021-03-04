using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class LevelData : ScriptableObject
{
    public int level;

    public BlockData[] availableBlocks; // o cambiar por el enumerator


    
    public Level levelPrefab;
}
