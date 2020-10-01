using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    /* TabGroup that owns this tab */
    public TabGroup tabGroup;

    private Image image;

    /* Images used when tab is selected or not selected */
    public Sprite selectedImage;
    public Sprite deselectedImage;

    /* Set to default tab */
    public bool isDefaultTab;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    private void Start()
    {
        if (isDefaultTab)
            Select();
    }

    private void Select()
    {
        tabGroup.OnSelected(this);
        image.sprite = selectedImage;
    }
    public void ResetTab()
    {
        image.sprite = deselectedImage;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}
