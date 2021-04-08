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
public class BoardState
{
    public int rows = 0;
    public int columns = 0;

    public BoardCellState[,] cells = { };
    public BoardObjectState[] boardElements = { };

    public BoardState(int rows, int cols, int nElements)
    {
        this.rows = rows;
        this.columns = cols;
        cells = new BoardCellState[rows, cols];
        boardElements = new BoardObjectState[nElements];
    }

    public void SetBoardObject(int i, BoardCell elementCell)
    {
        if (i > boardElements.Length) return;

        BoardObjectState objectState = new BoardObjectState();
        BoardObject element = elementCell.GetPlacedObject();
        if (element == null) return;

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
