using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "ScriptableObjects/Category")]
public class Category : ScriptableObject
{
    public string name_id;
    public string description;

    // public Level[] levels;
}
