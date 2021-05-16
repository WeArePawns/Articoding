using UnityEngine;
using UnityEngine.Events;

public enum TutorialType
{
    NONE,
    GENERAL,
    BOARD,
    CATEGORY_VARIABLES,
    CATEGORY_TYPES,
    CATEGORY_OPERATORS,
    CATEGORY_LOOPS,
    CATEGORY_CONDITIONS,
    ACTION
}


[CreateAssetMenu(fileName = "PopUpData", menuName = "ScriptableObjects/PopUpData")]
public class PopUpData : ScriptableObject
{
    public TutorialType type = TutorialType.NONE;
    public string title;
    [TextArea(3, 6)]
    public string content;
    public PopUpData next = null;
    public Sprite image;

    public PopUpData()
    {
    }

    public PopUpData(PopUpData data)
    {
        title = data.title;
        content = data.content;
        image = data.image;
    }
}
