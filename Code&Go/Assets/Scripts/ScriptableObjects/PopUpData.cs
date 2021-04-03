using UnityEngine;

[CreateAssetMenu(fileName = "PopUpData", menuName = "ScriptableObjects/PopUpData")]
public class PopUpData : ScriptableObject
{
    public string title;
    [TextArea(3, 6)]
    public string content;
    public Vector2 position;
    public Vector2 offset;
    public PopUpData next = null;

    public PopUpData()
    {
    }

    public PopUpData(PopUpData data)
    {
        title = data.title;
        content = data.content;
    }
}
