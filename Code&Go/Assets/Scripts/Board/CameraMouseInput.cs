using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMouseInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private OrbitCamera _cam;

    private Vector3 _prevMousePos;

    public bool mousePressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        mousePressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mousePressed = false;
    }

    void Update()
    {
        if (mousePressed)
        {
            // mouse movement in pixels this frame
            Vector3 mouseDelta = Input.mousePosition - _prevMousePos;

            // adjust to screen size
            Vector3 moveDelta = mouseDelta * (360f / Screen.height);

            _cam.Move(moveDelta.x, -moveDelta.y);
        }

        _prevMousePos = Input.mousePosition;
    }
}
