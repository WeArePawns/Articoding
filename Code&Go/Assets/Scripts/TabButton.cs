using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    /* TabGroup that owns this tab */
    public TabGroup tabGroup;

    private Image image;

    /* Images used when tab is selected or not selected */
    //public Sprite selectedImage;
    //public Sprite deselectedImage;

    Vector3 originalScale;

    public Color selectedColor;
    public Color deselectedColor;

    /* Set to default tab */
    public bool isDefaultTab;

    Vector3 newScale;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        originalScale = image.rectTransform.localScale;
        ResetTab();
    }

    private void Update()
    {
        image.rectTransform.localScale = Vector3.Lerp(image.rectTransform.localScale, newScale, 0.25f);
    }

    private void Start()
    {
        if (isDefaultTab)
            Select();
    }

    private void Select()
    {
        tabGroup.OnSelected(this);
        image.color = selectedColor;
        newScale = image.rectTransform.localScale * 1.5f;
    }
    public void ResetTab()
    {
        image.color = deselectedColor;
        newScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}
