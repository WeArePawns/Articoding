using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelControl : MonoBehaviour
{
    [SerializeField] private UnityEvent onDisable;
    [SerializeField] private GameObject panel;

    private bool disappearEnabled = false;

    private void Start()
    {
        if (panel == null)
            panel = gameObject;
    }

    private void Update()
    {
        if (panel.activeSelf)
            HideIfClickedOutside();
    }

    private void HideIfClickedOutside()
    {
        if (PopUpManager.Instance.IsShowing()) return;

        if (!disappearEnabled && Input.GetMouseButton(0) &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                GetComponent<RectTransform>(),
                Input.mousePosition,
                null))
        {
            panel.SetActive(false);
            onDisable.Invoke();
        }
    }

    public void Appear()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void DisableDissapear(bool enabled)
    {
        disappearEnabled = enabled;
    }
}
