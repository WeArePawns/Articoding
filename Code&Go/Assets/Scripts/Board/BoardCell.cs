using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardCell : MonoBehaviour
{
    protected static Dictionary<string, int> IDs = new Dictionary<string, int>();
    protected static int numIDs = 0;

    public enum BoardCellState
    {
        NONE,
        FREE,
        PARTIALLY_OCUPPIED,
        OCUPPIED
    }

    protected int x;
    protected int y;
    protected BoardManager boardManager;
    protected BoardObject placedObject;

    private BoardCellState state = BoardCellState.NONE;

    private void Awake()
    {
        SetState(BoardCellState.FREE);
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector3(x, 0.0f, y);
    }

    public void SetPosition(Vector2Int position)
    {
        x = position.x;
        y = position.y;
        transform.position = new Vector3(x, 0.0f, y);
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    public bool PlaceObject(BoardObject boardObject)
    {
        if (placedObject != null) return false; // Cannot place, cell ocuppied

        placedObject = boardObject;
        placedObject.transform.position = transform.position;
        SetState(BoardCellState.OCUPPIED);
        OnObjectPlaced();
        return true;
    }

    //Removes the reference to the object and optionally delete it
    public bool RemoveObject(bool deleteObject = true)
    {
        if (placedObject == null) return false; // Cannot remove, cell free

        if (deleteObject) Destroy(placedObject.gameObject);
        placedObject = null;
        SetState(BoardCellState.FREE);
        return true;
    }

    public BoardObject GetPlacedObject()
    {
        return placedObject;
    }

    public void SetState(BoardCellState state)
    {
        if (this.state == state) return;
        this.state = state;
    }

    public BoardCellState GetState()
    {
        return state;
    }

    public virtual int GetObjectID()
    {
        if (!IDs.ContainsKey(this.GetType().Name))
            IDs[this.GetType().Name] = numIDs++;
        return IDs[this.GetType().Name];
    }

    public void SetBoardManager(BoardManager board)
    {
        boardManager = board;
    }

    public abstract void OnObjectPlaced();

    public abstract string[] GetArgs();
}
