using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class ClickDetector : MonoBehaviour
{
    [SerializeField] private bool handleLeftClick = true;
    [SerializeField] private bool handleRightClick = true;
    [SerializeField] private bool handleMiddleClick = false;

    private LayerMask layerMask;    

    void Update()
    {
        GameObject clickedGmObj = null;
        IMouseListener mouseListener = null;
        // Left click 
        if (handleLeftClick && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)))
        {
            clickedGmObj = GetClickedGameObject();
            if (clickedGmObj != null)
            {
                mouseListener = clickedGmObj.GetComponent<IMouseListener>();
                if (mouseListener != null)
                    if (Input.GetMouseButtonDown(0))
                        mouseListener.OnMouseButtonDown(0);
                    else mouseListener.OnMouseButtonUp(0); ;
            }
        }
        // Right click 
        if (handleRightClick && (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1)))
        {
            clickedGmObj = GetClickedGameObject();
            if (clickedGmObj != null)
            {
                mouseListener = clickedGmObj.GetComponent<IMouseListener>();
                if (mouseListener != null)
                    if (Input.GetMouseButtonDown(1))
                        mouseListener.OnMouseButtonDown(1);
                    else mouseListener.OnMouseButtonUp(1); ;
            }
        }
        // Middle click 
        if (handleMiddleClick && (Input.GetMouseButtonDown(2) || Input.GetMouseButtonUp(2)))
        {
            clickedGmObj = GetClickedGameObject();
            if (clickedGmObj != null)
            {
                mouseListener = clickedGmObj.GetComponent<IMouseListener>();
                if (mouseListener != null)
                    if (Input.GetMouseButtonDown(2))
                        mouseListener.OnMouseButtonDown(2);
                    else mouseListener.OnMouseButtonUp(2); ;
            }
        }
    }

    GameObject GetClickedGameObject()
    {
        // Builds a ray from camera point of view to the mouse position 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            return hit.transform.gameObject;
        else
            return null;
    }
}