using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMouseInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private OrbitCamera _cam;
    private Vector3 _prevMousePos;

    private bool mousePressed;
    private bool draggingObject = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            mousePressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            mousePressed = false;
    }

    void Update()
    {
        if (mousePressed && !draggingObject)
        {
            // mouse movement in pixels this frame
            Vector3 mouseDelta = Input.mousePosition - _prevMousePos;

            // adjust to screen size
            Vector3 moveDelta = mouseDelta * (360f / Screen.height);

            _cam.Move(moveDelta.x, -moveDelta.y);
        }

        _prevMousePos = Input.mousePosition;
    }

    public void SetDragging(bool dragging)
    {
        draggingObject = dragging;
    }
}
