using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeZoom : MonoBehaviour
{
    [SerializeField] RectTransform codingArea;
    [SerializeField] float zoomAmount = 0.1f;
    [SerializeField] float minZoom = 0.5f;
    [SerializeField] float maxZoom = 1.5f;
    [SerializeField] GameObject zoomInButton;
    [SerializeField] GameObject zoomOutButton;
    [SerializeField] Color deactivatedColor;

    public void Zoom(bool zoomIn)
    {
        codingArea.localScale += zoomIn ? new Vector3(zoomAmount, zoomAmount, 0) : new Vector3(-zoomAmount, -zoomAmount, 0);
        CheckButton(zoomInButton, maxZoom, true);
        CheckButton(zoomOutButton, minZoom, false);
        codingArea.localScale = new Vector3(Mathf.Clamp(codingArea.localScale.x, minZoom, maxZoom), Mathf.Clamp(codingArea.localScale.y, minZoom, maxZoom), codingArea.localScale.z);
    }

    private void CheckButton(GameObject button, float scale, bool greater)
    {
        Image zoomImage = button.GetComponent<Image>();
        Button zoomButton = button.GetComponent<Button>();
        bool deactiveate = (greater && (codingArea.localScale.x > scale || codingArea.localScale.y > scale)) || (!greater && (codingArea.localScale.x < scale || codingArea.localScale.y < scale));
        if (zoomImage != null)
            zoomImage.color = deactiveate ? deactivatedColor : Color.white;
        if (zoomButton != null)
            zoomButton.enabled = !deactiveate;
    }

    public void ResetZoom()
    {
        codingArea.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        CheckButton(zoomInButton, maxZoom, true);
        CheckButton(zoomOutButton, minZoom, false);
    }
}
