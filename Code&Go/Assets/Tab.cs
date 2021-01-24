using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class Tab : MonoBehaviour, IPointerClickHandler
{
    [Header("Graphics")]
    [SerializeField] Text text;
    [SerializeField] Image image;

    [Header("Text colors")]
    [SerializeField] Color textSelectedColor;
    [SerializeField] Color textDeselectedColor;

    [Header("Image colors")]
    [SerializeField] Color imageSelectedColor;
    [SerializeField] Color imageDeselectedColor;

    [System.Serializable]
    public struct TabCallbacks
    {
        public UnityEvent OnSelected;
        public UnityEvent OnDeselected;
    }

    [Space]
    public TabCallbacks callbacks;

    private bool selected = false;

#if !UNITY_EDITOR
    private void Awake()
    {
        Configure();
    }
#else
    private void Update()
    {
        Configure();
    }
#endif

    private void Configure()
    {
        if (!selected)
        {
            text.color = textDeselectedColor;
            image.color = imageDeselectedColor;
        }
        else
        {
            text.color = textSelectedColor;
            image.color = imageSelectedColor;
        }
    }

    public void Select()
    {
        selected = true;

        if (callbacks.OnSelected != null)
            callbacks.OnSelected.Invoke();

        text.color = textSelectedColor;
        image.color = imageSelectedColor;
    }

    public void Deselect()
    {
        selected = false;

        if (callbacks.OnDeselected != null)
            callbacks.OnDeselected.Invoke();

        text.color = textDeselectedColor;
        image.color = imageDeselectedColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}
