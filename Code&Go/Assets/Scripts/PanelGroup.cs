using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelGroup : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /* Public atributes */
    public TabGroup tabGroup;
    [Space()]
    [Min(0.0f)]
    public float timeSpan;

    /* Private atributes */
    /* Current shown panel index */
    private int currentPanel;

    /* Auxiliar variables */
    private RectTransform rect;
    private float panelWidth;
    private Vector2 startPosition;
    private Vector2 desiredPosition;
    private float timeAccumulator;

    private bool isDragging;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        panelWidth = rect.rect.width;
    }

    private void ShowCurrentPanel()
    {
        float offset = rect.rect.width / 2.0f - panelWidth / 2.0f;

        // Set point A and B
        startPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
        desiredPosition = new Vector2(currentPanel * -panelWidth + offset, rect.anchoredPosition.y);

        // Set timer
        timeAccumulator = timeSpan;
    }

    private void Update()
    {
        DoInterpolation();
        CheckCurrentPage();
    }

    public void SetPanelIndex(int index)
    {
        currentPanel = index;
        ShowCurrentPanel();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        ShowCurrentPanel();
    }

    private void DoInterpolation()
    {
        float timeAux = timeAccumulator;
        timeAccumulator -= Time.deltaTime;
        if (!isDragging && (timeAccumulator >= 0.0f || timeAux > 0.0f)) // Si TimeAux es positivo, el ratio tiene que llegar a 1 por ultima vez
        {
            float ratio = 1.0f - timeAccumulator / timeSpan;
            rect.anchoredPosition = Vector2.Lerp(startPosition, desiredPosition, ratio);
        }
    }

    private void CheckCurrentPage()
    {
        if (isDragging)
        {
            float offset = rect.rect.width / 2.0f;
            float xPosition = rect.anchoredPosition.x - offset;

            int index = -(int)xPosition / (int)panelWidth;
            index = Mathf.Clamp(index, 0, transform.childCount - 1);

            if(index != currentPanel)
            {
                currentPanel = index;
                tabGroup.SetTabIndex(currentPanel);
            }
        }
    }

}
