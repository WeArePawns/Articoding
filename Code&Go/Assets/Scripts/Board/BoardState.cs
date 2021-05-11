using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BoardCellState
{
    public int id;
    public int x;
    public int y;
    public string[] args;
}

[System.Serializable]
public class BoardObjectState
{
    public int id;
    public int orientation;
    public int x;
    public int y;
    public string [] args;
}

[System.Serializable]
public class BoardHintState
{
    public int id;
    public int orientation;
    public int amount;
    public int x;
    public int y;
}

[System.Serializable]
public class BoardState
{
    public int rows = 0;
    public int columns = 0;

    public BoardCellState[] cells = { };
    public BoardObjectState[] boardElements = { };    
    public BoardHintState[] boardHints = { };    

    public BoardState(int rows, int cols, int nElements, int nHints)
    {
        this.rows = rows;
        this.columns = cols;
        cells = new BoardCellState[rows * cols];
        boardElements = new BoardObjectState[nElements];
        boardHints = new BoardHintState[nHints];
    }

    public void SetBoardCell(BoardCell cell)
    {
        Vector2Int pos = cell.GetPosition();
        int x = pos.x;
        int y = pos.y;

        // Board cell
        BoardCellState cellState = new BoardCellState();
        cellState.id = cell.GetObjectID();
        cellState.x = x;
        cellState.y = y;
        cellState.args = cell.GetArgs();

        cells[(x-1) + (y-1) * columns] = cellState;
    }

    public void SetBoardHint(int i, BoardHint hint)
    {
        BoardHintState hintState = new BoardHintState();
        hintState.id = hint.GetHintID();
        hintState.x = hint.GetPosition().x;
        hintState.y = hint.GetPosition().y;
        hintState.amount = hint.GetAmount();
        hintState.orientation = (int)hint.GetDirection();
        boardHints[i] = hintState;
    }

    public void SetBoardObject(int i, BoardCell elementCell)
    {
        if (i >= boardElements.Length) return;

        BoardObject element = elementCell.GetPlacedObject();
        if (element == null) return;

        BoardObjectState objectState = new BoardObjectState();
        objectState.id = element.GetObjectID();
        Vector2Int pos = elementCell.GetPosition();
        objectState.x = pos.x;
        objectState.y = pos.y;
        objectState.args = element.GetArgs();
        objectState.orientation = (int)element.GetDirection();

        boardElements[i] = objectState;
    }

    public static BoardState FromJson(string text)
    {
        return JsonUtility.FromJson<BoardState>(text);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
