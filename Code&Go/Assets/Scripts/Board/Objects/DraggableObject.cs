using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IMouseListener
{
    private BoardManager board;
    private ArgumentLoader argumentLoader = null;
    private BoardObject boardObject;

    private Vector3 mouseOffset;
    Vector2Int lastPos = -Vector2Int.one;
    private float zCoord;
    private bool dragging = false;

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void Start()
    {
        boardObject = GetComponent<BoardObject>();
    }

    private void Update()
    {
        if (dragging)
        {
            transform.position = GetMouseWorldPos() + mouseOffset;
            if (Input.GetMouseButtonUp(0))
                OnLeftUp();
        }
    }

    public void SetBoard(BoardManager board)
    {
        this.board = board;
    }

    public void SetArgumentLoader(ArgumentLoader loader)
    {
        this.argumentLoader = loader;
    }

    private void OnLeftDown()
    {
        if (boardObject == null) boardObject = GetComponent<BoardObject>();
        argumentLoader.SetBoardObject(boardObject);

        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        mouseOffset = transform.position - GetMouseWorldPos();
        dragging = true;
    }

    private void OnRightDown()
    {
        if (boardObject == null) boardObject = GetComponent<BoardObject>();
        argumentLoader.SetBoardObject(boardObject);

        boardObject.Rotate(1);
    }

    private void OnLeftUp()
    {
        if (dragging)
        {
            dragging = false;
            Vector3 pos = board.GetLocalPosition(transform.position);
            pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
            if (boardObject != null && pos.x < board.GetColumns() && pos.x >= 0 && pos.z < board.GetRows() && pos.z >= 0)
            {
                if (pos.x == lastPos.x && pos.z == lastPos.y)
                    transform.localPosition = new Vector3(lastPos.x, transform.position.y, lastPos.y);
                else if (board.GetBoardCellType((int)pos.x, (int)pos.z) == 1 || !board.AddBoardObject((int)pos.x, (int)pos.z, boardObject))
                    Destroy(gameObject, 0.1f);
                board.RemoveBoardObject(lastPos.x, lastPos.y, false);
                lastPos = new Vector2Int((int)pos.x, (int)pos.z);
            }
            else
                Destroy(gameObject, 0.1f);
        }
    }

    public void OnMouseButtonDown(int index)
    {
        if (index == 0)
            OnLeftDown();
        else if (index == 1)
            OnRightDown();
    }

    public void OnMouseButtonUp(int index)
    {

    }
}
