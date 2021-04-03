using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PopUpData", menuName = "ScriptableObjects/PopUpData")]
public class PopUpData : ScriptableObject
{
    public string title;
    [TextArea(3, 6)]
    public string content;
    public PopUpData next = null;
    public UnityEvent OnButtonPressed;

    public PopUpData()
    {
    }

    public PopUpData(PopUpData data)
    {
        title = data.title;
        content = data.content;
    }
}
