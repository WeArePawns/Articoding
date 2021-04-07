using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    /* Public atributes */
    public TabGroup tabGroup;

    [Space()]
    [Min(0.0f)]
    public float timeSpan;
    [Min(0.0f)]
    public float scaleRatio;

    [Space()]
    public Color selectedColor;
    public Color deselectedColor;
    public Color hoverColor;

    [Space()]
    /* Set to default tab */
    public bool isDefaultTab;

    /* Private atributes */
    private Image image;
    private Vector3 originalScale;

    /* Auxiliar variables */
    private float timeAccumulator;
    private Vector3 startScale;
    private Vector3 desiredScale;


    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        originalScale = image.rectTransform.localScale;
        ResetTab();
    }

    private void Start()
    {
        if (isDefaultTab)
            Select();
    }

    private void Update()
    {
        DoInterpolation();
    }

    public void Select()
    {
        tabGroup.OnSelected(this);
        image.color = selectedColor;

        // Set point A and B
        startScale = image.rectTransform.localScale;
        desiredScale = originalScale * scaleRatio;
        // Set timer
        timeAccumulator = timeSpan;
    }

    public void ResetTab()
    {
        image.color = deselectedColor;

        // Set point A and B
        startScale = image.rectTransform.localScale;
        desiredScale = originalScale;
        // Set timer
        timeAccumulator = timeSpan;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = deselectedColor;
    }

    private void DoInterpolation()
    {
        float timeAux = timeAccumulator;
        timeAccumulator -= Time.deltaTime;
        if (timeAccumulator >= 0.0f || timeAux > 0.0f) // Si TimeAux es positivo, el ratio tiene que llegar a 1 por ultima vez
        {
            float ratio = 1.0f - timeAccumulator / timeSpan;
            image.rectTransform.localScale = Vector3.Lerp(startScale, desiredScale, ratio);
        }
    }
}
