using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Category", menuName = "ScriptableObjects/Category")]
public class Category : ScriptableObject
{
    public LocalizedString nameIDLocalized;
    public LocalizedString descriptionLocalized;

    public string name_id;
    [TextArea(3, 6)]
    public string description;

    public List<LevelData> levels;

    public Sprite icon;
}
