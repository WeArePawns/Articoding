using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelGroup : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /*
     * Esta es la clase que se modificará para cambiar
     * la forma en la que se cambian los paneles al 
     * deslizar con gestos.
     */


    /* All panels */
    public GameObject[] panels;

    private int currentPanel;

    RectTransform rect;
    Vector2 newPosition;

    public bool dragging = false;
    float panelWidth;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void ShowCurrentPanel()
    {
        /*for(int i = 0; i < panels.Length; i++)
        {
            bool shown = (i == currentPanel);
            panels[i].SetActive(shown);
        }*/

        panelWidth = rect.rect.width / panels.Length;

        newPosition = new Vector2(currentPanel * -panelWidth + rect.rect.width / 2 - panelWidth / 2, rect.anchoredPosition.y);

        //GetComponent<RectTransform>() = rect;
    }

    private void Update()
    {
        if (rect.anchoredPosition != newPosition && !dragging)
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, newPosition, 0.25f);

        // Swipe
        if (dragging)
        {
            if (rect.anchoredPosition.x > newPosition.x - panelWidth / 2)
                SetPanelIndex(currentPanel - 1);
            else if (rect.anchoredPosition.x < newPosition.x + panelWidth / 2)
                SetPanelIndex(currentPanel + 1);
        }

        print(currentPanel);
    }

    public void SetPanelIndex(int index)
    {
        currentPanel = index;
        ShowCurrentPanel();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
