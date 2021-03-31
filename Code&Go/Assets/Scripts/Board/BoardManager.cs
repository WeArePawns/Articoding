using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int rows;
    private int columns;
    private int nReceivers = 0;
    private int nReceiversActive = 0;

    [SerializeField] private BoardCell cellPrefab;
    [SerializeField] private Transform cellsParent;
    [SerializeField] private Transform elementsParent;
    [SerializeField] private BoardObject[] elements;

    // Hidden atributtes
    private BoardCell[,] board;

    private void Awake()
    {
        InitIDs();
    }

    public void GenerateBoard()
    {
        // If a board already exist, destroy it
        DestroyBoard();
        // Initialize board
        board = new BoardCell[columns, rows];
        // Instantiate cells
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                BoardCell cell = Instantiate(cellPrefab, cellsParent);
                cell.SetPosition(x, y);
                board[x, y] = cell;
            }
        }
        nReceivers = 0;
        nReceiversActive = 0;
    }

    private void DestroyBoard()
    {
        foreach (Transform child in cellsParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in elementsParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void InitIDs()
    {
        foreach (BoardObject element in elements)
            element.GetObjectID();
    }


    private bool IsInBoardBounds(int x, int y)
    {
        return y >= 0 && y < rows && x >= 0 && x < columns;
    }
    private bool IsInBoardBounds(Vector2Int position)
    {
        return IsInBoardBounds(position.x, position.y);
    }

    public void LoadBoard(TextAsset textAsset)
    {
        if (textAsset == null) return;

        BoardState state = BoardState.FromJson(textAsset.text);
        rows = state.rows;
        columns = state.columns;
        GenerateBoard();

        //Generate board elements
        foreach (BoardObjectState objectState in state.boardElements)
            AddBoardObject(objectState.id, objectState.x, objectState.y, objectState.orientation, objectState.args);
    }

    public void RegisterReceiver()
    {
        nReceivers++;
    }

    public void ReceiverActivated()
    {
        nReceiversActive++;
    }

    public void ReceiverDeactivated()
    {
        nReceiversActive--;
    }

    public bool BoardCompleted()
    {
        return nReceivers > 0 && nReceiversActive >= nReceivers;
    }

    public void SetRows(int rows)
    {
        this.rows = rows;
    }

    public void SetColumns(int columns)
    {
        this.columns = columns;
    }

    public int GetRows()
    {
        return rows;
    }

    public int GetColumns()
    {
        return columns;
    }

    public BoardCell GetBoardCell(int x, int y)
    {
        return IsInBoardBounds(x, y) ? board[x, y] : null;
    }

    public string GetBoardState()
    {
        BoardState state = new BoardState(rows, columns, elementsParent.childCount);
        int i = 0;
        foreach (BoardCell cell in board)
            if (i < elementsParent.childCount && cell.GetState() == BoardCell.BoardCellState.OCUPPIED)
                state.SetBoardObject(i++, cell);

        return state.ToJson();
    }

    public void AddBoardObject(int id, int x, int y, int orientation = 0, string[] additionalArgs = null)
    {
        if (id > elements.Length) return;

        BoardObject bObject = Instantiate(elements[id], elementsParent);
        bObject.SetBoard(this);
        bObject.SetDirection((BoardObject.Direction)orientation);
        bObject.LoadArgs(additionalArgs);
        AddBoardObject(x, y, bObject);
    }

    public void AddBoardObject(int x, int y, BoardObject boardObject)
    {
        if (IsInBoardBounds(x, y) && boardObject != null)
            board[x, y].PlaceObject(boardObject);
    }

    public void RemoveBoardObject(int x, int y)
    {
        if (IsInBoardBounds(x, y))
            board[x, y].RemoveObject();
    }

    public void MoveBoardObject(Vector2Int from, Vector2Int to)
    {
        if (IsInBoardBounds(from) && IsInBoardBounds(to) && from != to)
        {
            BoardCell cell = board[to.x, to.y];
            if (cell.GetState() != BoardCell.BoardCellState.FREE) return;


            BoardObject bObject = board[from.x, from.y].GetPlacedObject();
            if (bObject == null) return;

            board[from.x, from.y].RemoveObject(false);
            board[to.x, to.y].PlaceObject(bObject);
        }
    }

    public void RotateBoardObject(Vector2Int position, int direction)
    {
        if (IsInBoardBounds(position))
        {
            BoardObject bObject = board[position.x, position.y].GetPlacedObject();
            if (bObject == null) return;

            bObject.Rotate(direction);
        }
    }

    public void SetBoardObjectDirection(Vector2Int position, BoardObject.Direction direction)
    {
        if (IsInBoardBounds(position))
        {
            BoardObject bObject = board[position.x, position.y].GetPlacedObject();
            if (bObject == null) return;

            bObject.SetDirection(direction);
        }
    }

    // Smooth interpolation, this code must be called by blocks
    public void MoveObject(Vector2Int origin, Vector2Int direction, float time)
    {
        // Check valid direction
        if (!(direction.magnitude == 1.0f && (Mathf.Abs(direction.x) == 1 || Mathf.Abs(direction.y) == 1)))
            return;

        // Check if origin object exists
        if (!IsInBoardBounds(origin)) return;

        BoardObject bObject = board[origin.x, origin.y].GetPlacedObject();
        if (bObject == null) return; // No object to move

        // Check if direction in valid
        if (IsInBoardBounds(origin + direction))
        {
            // A valid direction
            BoardCell fromCell = board[origin.x, origin.y];
            BoardCell toCell = board[origin.x + direction.x, origin.y + direction.y];

            if (toCell.GetPlacedObject() != null) return;

            StartCoroutine(InternalMoveObject(fromCell, toCell, time));
        }
        else
        {
            // Not a valid direction
            // TODO: hacer que se choque o haga algo
        }
    }

    public void RotateObject(Vector2Int origin, int direction, float time)
    {
        // Check direction
        if (direction == 0) return;
        direction = direction < 0 ? -1 : 1;

        // Check if origin object exists
        if (!IsInBoardBounds(origin)) return;

        BoardObject bObject = board[origin.x, origin.y].GetPlacedObject();
        if (bObject == null) return; // No to rotate

        // You cannot rotate an object that is already rotating
        if (bObject.GetDirection() == BoardObject.Direction.PARTIAL) return;

        StartCoroutine(InternalRotateObject(bObject, direction, time));
    }

    // We assume, all are valid arguments
    private IEnumerator InternalMoveObject(BoardCell from, BoardCell to, float time)
    {
        // All cells to PARTIALLY_OCUPPIED
        from.SetState(BoardCell.BoardCellState.PARTIALLY_OCUPPIED);
        to.SetState(BoardCell.BoardCellState.PARTIALLY_OCUPPIED);

        BoardObject bObject = from.GetPlacedObject();

        Vector2 fromPos = bObject.transform.position;
        Vector2 toPos = to.transform.position;

        // Interpolate movement
        float auxTimer = 0.0f;
        while (auxTimer < time)
        {
            Vector2 lerpPos = Vector2.Lerp(fromPos, toPos, auxTimer / time);
            bObject.transform.position = lerpPos;
            auxTimer += Time.deltaTime;
            yield return null;
        }

        // Replace
        from.RemoveObject(false);
        to.PlaceObject(bObject);

        // All cells to correct state
        from.SetState(BoardCell.BoardCellState.FREE);
        to.SetState(BoardCell.BoardCellState.OCUPPIED);
    }

    private IEnumerator InternalRotateObject(BoardObject bObject, int direction, float time)
    {
        // Get direction
        BoardObject.Direction currentDirection = bObject.GetDirection();
        BoardObject.Direction targetDirection = (BoardObject.Direction)((((int)currentDirection + direction) + 8) % 8);

        // Froze direction
        bObject.SetDirection(BoardObject.Direction.PARTIAL, false);

        Vector3 fromAngles = bObject.transform.localEulerAngles;
        fromAngles.z = (int)currentDirection * -45.0f;
        Vector3 toAngles = bObject.transform.localEulerAngles;
        toAngles.z = ((int)currentDirection + direction) * -45.0f;

        // Interpolate movement
        float auxTimer = 0.0f;
        while (auxTimer < time)
        {
            Vector3 lerpAngles = Vector3.Lerp(fromAngles, toAngles, auxTimer / time);
            bObject.transform.localEulerAngles = lerpAngles;
            auxTimer += Time.deltaTime;
            yield return null;
        }

        // Set final rotation (defensive code)
        bObject.SetDirection(targetDirection);
    }
}
