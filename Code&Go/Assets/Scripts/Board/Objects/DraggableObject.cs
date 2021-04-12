using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private BoardManager board;
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
            transform.position = GetMouseWorldPos() + mouseOffset;
    }

    public void SetBoard(BoardManager board)
    {
        this.board = board;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
            mouseOffset = transform.position - GetMouseWorldPos();
            dragging = true;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
            boardObject.Rotate(1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            dragging = false;
            Vector3 pos = board.GetLocalPosition(transform.position);
            pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
            if (boardObject != null && pos.x < board.GetColumns() && pos.x >= 0 && pos.y < board.GetRows() && pos.y >= 0)
            {

                if (pos.x == lastPos.x && pos.y == lastPos.y)
                    transform.localPosition = new Vector3(lastPos.x, lastPos.y, transform.position.z);
                else if (!board.AddBoardObject((int)pos.x, (int)pos.y, boardObject))
                    Destroy(gameObject, 0.1f);
                board.RemoveBoardObject(lastPos.x, lastPos.y, false);
                lastPos = new Vector2Int((int)pos.x, (int)pos.y);
            }
            else
                Destroy(gameObject, 0.1f);
        }
    }
}
