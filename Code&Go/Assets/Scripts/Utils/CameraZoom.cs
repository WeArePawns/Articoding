using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool inside = false;

    public Camera cam;

    public float minFov, maxFov;
    public float sensitivity;

    private float fov;
    private float iniFov = -1.0f;

    public void Reset()
    {
        fov = iniFov;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        inside = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        inside = false;
    }

    void Update()
    {
        if (!inside) return;

        if(iniFov == -1.0)
        {
            fov = cam.orthographicSize;
            iniFov = fov;
        }

        // Zoom con la rueda del ratón
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
    }

    private void LateUpdate()
    {
        if (iniFov == -1.0) return;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, fov, Time.deltaTime * 3.0f);
    }

    public void ResetInmediate()
    {
        Reset();
        cam.orthographicSize = fov;
    }
}