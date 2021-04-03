using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopUpTrigger : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private PopUpData data;
    [SerializeField] private bool destroyOnTriggered = false;
    [SerializeField] private UnityEvent OnTriggered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PopUpManager.Instance.IsShowing()) return;

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
       // if (PopUpManager.Instance.IsShowing()) return;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

        //Set the Pointer Event Position to that of the mouse position
        pointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        GraphicRaycaster gRaycaster =  PopUpManager.Instance.GetGraphicRaycaster();
        gRaycaster.Raycast(pointerEventData, results);

        if (results.Count != 0) return; // Some kind of UI over trigger

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
