using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private int rows;
    private int columns;

    [SerializeField] private BoardCell cellPrefab;
    [SerializeField] private Transform cellsParent;
    [SerializeField] private Transform elementsParent;
    [SerializeField] private BoardObject[] elements;

    [SerializeField] private TextAsset boardState;

    // Hidden atributtes
    private BoardCell[,] board;

    private void Awake()
    {
        InitIDs();
        LoadBoard(boardState);
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


    private void LoadBoard(TextAsset textAsset)
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
    private bool IsInBoardBounds(int x, int y)
    {
        return y >= 0 && y < rows && x >= 0 && x < columns;
    }
    private bool IsInBoardBound(Vector2Int position)
    {
        return IsInBoardBounds(position.x, position.y);
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
        if (IsInBoardBound(from) && IsInBoardBound(to) && from != to)
        {
            BoardObject bObject = board[from.x, from.y].GetPlacedObject();
            if (bObject == null) return;

            board[from.x, from.y].RemoveObject(false);
            board[to.x, to.y].PlaceObject(bObject);
        }
    }

    public void RotateBoardObject(Vector2Int position, int direction)
    {
        if (IsInBoardBound(position))
        {
            BoardObject bObject = board[position.x, position.y].GetPlacedObject();
            if (bObject == null) return;

            bObject.Rotate(direction);
        }
    }

    public void SetBoardObjectDirection(Vector2Int position, BoardObject.Direction direction)
    {
        if (IsInBoardBound(position))
        {
            BoardObject bObject = board[position.x, position.y].GetPlacedObject();
            if (bObject == null) return;

            bObject.SetDirection(direction);
        }
    }
}
