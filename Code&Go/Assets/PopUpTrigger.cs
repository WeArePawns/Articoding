using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PopUpTrigger : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private PopUpData data;
    [SerializeField] private bool destroyOnTriggered = false;
    [SerializeField] private UnityEvent OnTriggered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show
        RectTransform rectTransform = GetComponent<RectTransform>();
        PopUpManager.Instance.Show(data, RectUtils.RectTransformToScreenSpace(rectTransform));

        if (OnTriggered != null)
            OnTriggered.Invoke();

        // Destroy
        if(destroyOnTriggered)
            Destroy(this);
    }

    public void OnMouseEnter()
    {
        // Show
        Collider collider = GetComponent<Collider>();
        PopUpManager.Instance.Show(data, RectUtils.ColliderToScreenSpace(collider));

        if (OnTriggered != null)
            OnTriggered.Invoke();

        // Destroy
        if (destroyOnTriggered)
            Destroy(this);
    }

}
