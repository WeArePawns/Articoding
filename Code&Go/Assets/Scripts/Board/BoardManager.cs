using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private void GenerateBoard()
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

    public BoardCell GetBoardCell(int x, int y)
    {
        if (y < 0 || y > rows || x < 0 || x > columns)
            return null;
        return board[x, y];
    }

    public int GetRows()
    {
        return rows;
    }

    public int GetColumns()
    {
        return columns;
    }

    public string GetBoardState()
    {
        BoardState state = new BoardState(rows, columns, elementsParent.childCount);

        int i = 0;
        foreach (BoardCell cell in board)
        {
            if (i < elementsParent.childCount && cell.GetState() == BoardCell.BoardCellState.OCUPPIED)
                state.SetBoardObject(i++, cell);
        }

        return state.ToJson();
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
        {
            BoardObject bObject = Instantiate(elements[objectState.id], elementsParent);
            bObject.SetBoard(this);
            bObject.SetDirection((BoardObject.Direction)objectState.orientation);
            bObject.LoadArgs(objectState.args);
            board[objectState.x, objectState.y].PlaceObject(bObject);
        }
    }

}
