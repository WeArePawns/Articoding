using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "ScriptableObjects/Category")]
public class Category : ScriptableObject
{
    public string name_id;
    [TextArea(3, 6)]
    public string description;

    public LevelData[] levels;

    public Sprite icon;
}
