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
    private float iniFov;

    private void Start()
    {
        fov = cam.orthographicSize;
        iniFov = fov;
    }

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

        // Zoom con la rueda del ratón
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
    }

    private void LateUpdate()
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, fov, Time.deltaTime * 3.0f);
    }
}