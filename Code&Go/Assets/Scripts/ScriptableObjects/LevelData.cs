using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class LevelData : ScriptableObject
{

    public LocalizedString levelNameLocalized;
    public string levelName;
    public Sprite levelPreview;

    public LocalizedAsset<TextAsset> initialState; // Estado inicial en .xml

    [Header("Active Blocks")]
    public TextAsset activeBlocks;//Bloques y categorias disponibles    
    public bool allActive = false;

    [Space(10)]
    public TextAsset levelBoard;
    public string auxLevelBoard;

    public int minimosPasos;
}
