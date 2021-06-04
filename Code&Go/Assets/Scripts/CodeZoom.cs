using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CodeZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform codingArea;
    [SerializeField] float zoomAmount = 0.1f;
    [SerializeField] float minZoom = 0.5f;
    [SerializeField] float maxZoom = 1.5f;
    [SerializeField] Button zoomInButton;
    [SerializeField] Button zoomOutButton;
    [SerializeField] Button zoomResetButton;
    [SerializeField] Color deactivatedColor;

    private bool inside = false;

    private void Awake()
    {
        zoomInButton.onClick.AddListener(() => Zoom(true));
        zoomOutButton.onClick.AddListener(() => Zoom(false));
        zoomResetButton.onClick.AddListener(() => ResetZoom());
    }

    private void Update()
    {
        if (inside)
        {
            float delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta != 0.0f)
                Zoom(delta > 0);
        }
    }

    public void Zoom(bool zoomIn)
    {
        codingArea.localScale += zoomIn ? new Vector3(zoomAmount, zoomAmount, 0) : new Vector3(-zoomAmount, -zoomAmount, 0);
        CheckButton(zoomInButton, maxZoom, true);
        CheckButton(zoomOutButton, minZoom, false);
        codingArea.localScale = new Vector3(Mathf.Clamp(codingArea.localScale.x, minZoom, maxZoom), Mathf.Clamp(codingArea.localScale.y, minZoom, maxZoom), codingArea.localScale.z);
    }

    private void CheckButton(Button button, float scale, bool greater)
    {
        bool deactiveate = (greater && (codingArea.localScale.x > scale || codingArea.localScale.y > scale)) || (!greater && (codingArea.localScale.x < scale || codingArea.localScale.y < scale));
        if (button != null)
            button.interactable = !deactiveate;
    }

    public void ResetZoom()
    {
        codingArea.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        CheckButton(zoomInButton, maxZoom, true);
        CheckButton(zoomOutButton, minZoom, false);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        inside = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        inside = false;
    }

}
