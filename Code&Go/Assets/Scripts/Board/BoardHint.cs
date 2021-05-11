using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHint : MonoBehaviour
{
    [SerializeField] private GameObject[] hints;

    protected int x;
    protected int y;
    protected BoardObject.Direction orientation = BoardObject.Direction.RIGHT;
    protected int amount = 1;
    protected int id;

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.localPosition = new Vector3(x, -0.49f, y);
    }

    public void SetPosition(Vector2Int position)
    {
        x = position.x;
        y = position.y;
        transform.localPosition = new Vector3(x, -0.49f, y);
    }
    public void SetDirection(BoardObject.Direction direction, bool rotate = true)
    {
        orientation = direction;

        if (!rotate) return;

        if (direction == BoardObject.Direction.PARTIAL)
            direction = BoardObject.Direction.RIGHT; // Default

        Vector3 lastRot = transform.localEulerAngles;
        lastRot.y = (int)direction * 45.0f;
        transform.localEulerAngles = lastRot;
    }

    public void Rotate(int dir)
    {
        BoardObject.Direction newDir = (BoardObject.Direction)((((int)orientation + dir) + 8) % 8);
        SetDirection(newDir);
    }

    public void SetAmount(int amount)
    {
        this.amount = amount;
    }

    public void SetHintID(int id)
    {
        if (id < 0 || id >= hints.Length) return;
        this.id = id;
        hints[id].SetActive(true);
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    public int GetHintID()
    {
        return id;
    }

    public int GetAmount()
    {
        return amount;
    }

    public BoardObject.Direction GetDirection()
    {
        return orientation;
    }
}
