using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
    private int x;
    private int y;
    private GameObject placedObject;

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector2(x, y);
    }

    public void SetPosition(Vector2Int position)
    {
        x = position.x;
        y = position.y;
        transform.position = new Vector2(x, y);
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    public bool PlaceObject(GameObject gameObject)
    {
        if (placedObject != null) return false; // Cannot placed, cell ocuppied

        placedObject = gameObject;
        return true;
    }
}
