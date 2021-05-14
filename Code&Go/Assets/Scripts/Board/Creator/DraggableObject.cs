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
    private bool modifiable = true;

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
        if (modifiable && dragging)
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
        if (!modifiable) return;
        if (boardObject == null) boardObject = GetComponent<BoardObject>();
        if (argumentLoader != null) argumentLoader.SetBoardObject(boardObject);

        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        mouseOffset = transform.position - GetMouseWorldPos();
        dragging = true;
    }

    private void OnRightDown()
    {
        if (!modifiable) return;
        if (boardObject == null) boardObject = GetComponent<BoardObject>();
        if (argumentLoader != null) argumentLoader.SetBoardObject(boardObject);

        boardObject.Rotate(1);
    }

    private void OnLeftUp()
    {
        if (modifiable && dragging)
        {
            dragging = false;
            Vector3 pos = board.GetLocalPosition(transform.position);
            pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
            if (boardObject != null && pos.x < board.GetColumns() && pos.x >= 0 && pos.z < board.GetRows() && pos.z >= 0)
            {
                Vector2Int newPos = new Vector2Int((int)pos.x, (int)pos.z);
                //Si la posicion en la que se suelta es donde estaba colocado no se hace nada
                if (lastPos == newPos)
                {
                    transform.localPosition = new Vector3(lastPos.x, 0, lastPos.y);
                    return;
                }
                //Si se suelta en una celda ocupada o en un agujero se elimina
                if (board.GetBoardCellType(newPos.x, newPos.y) == 1 || board.IsCellOccupied(newPos.x, newPos.y))
                {
                    //Se elimina el objeto en la posicion anterior
                    board.RemoveBoardObject(lastPos.x, lastPos.y);
                    Destroy(gameObject, 0.3f);
                    return;
                }
                //Si el objeto no se ha añadido al tablero
                if (lastPos == -Vector2Int.one)
                    board.AddBoardObject(newPos.x, newPos.y, boardObject);
                else//Se mueve el objeto
                    board.MoveBoardObject(lastPos, newPos);
                lastPos = newPos;
            }
            else
            {
                board.RemoveBoardObject(lastPos.x, lastPos.y);
                Destroy(gameObject, 0.3f);
            }
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

    public void SetLastPos(Vector2Int lastPos)
    {
        this.lastPos = lastPos;
    }

    public void SetModifiable(bool modifiable)
    {
        this.modifiable = modifiable;
    }
}
