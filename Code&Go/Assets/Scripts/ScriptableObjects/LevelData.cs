using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class LevelData : ScriptableObject
{
    public TextAsset statement; // Enunciado en .xml
    public TextAsset initialState; // Estado inicial en .xml

    public BlockData[] availableBlocks; // o cambiar por el enumerator

    public Level levelPrefab;
}
