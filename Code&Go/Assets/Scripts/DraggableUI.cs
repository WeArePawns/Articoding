using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    public bool freezeX;
    public bool freezeY;

    public UnityEvent onBeginDrag;
    public UnityEvent onDrag;
    public UnityEvent onEndDrag;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (canvas == null)
        {
            Transform canvasTransform = transform;
            while(canvasTransform != null)
            {
                canvas = canvasTransform.GetComponent<Canvas>();
                if (canvas != null)
                    break;
                canvasTransform = canvasTransform.parent;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = rectTransform.anchoredPosition;

        if (!freezeX) position.x += eventData.delta.x / canvas.scaleFactor;
        if (!freezeY) position.y += eventData.delta.y / canvas.scaleFactor;

        rectTransform.anchoredPosition = position;

        if(onDrag != null) onDrag.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag.Invoke();
    }
}