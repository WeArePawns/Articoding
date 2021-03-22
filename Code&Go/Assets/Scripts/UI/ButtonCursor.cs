using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCursor : MonoBehaviour
{
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Texture2D pressCursor;
    [SerializeField] private Texture2D viewCursor;

    void Start()
    { 

    }

    void OnMouseOver()
    {
        Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }
}
