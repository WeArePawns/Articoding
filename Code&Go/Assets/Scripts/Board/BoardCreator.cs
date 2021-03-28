using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] BoardManager board;

    private string fileName = "level";
    private int nLevel = 1;
    private string filePath = "";

    private Vector2Int cursorPos = Vector2Int.zero;
    private Vector2 offset = new Vector2(-0.3f, 0.3f);
    private BoardObject selectedObject = null;

    int rows, columns;

    private void Start()
    {
        filePath = Application.dataPath + "/LevelsCreated/";
        rows = board.GetRows();
        columns = board.GetColumns();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            using (StreamWriter outputFile = new StreamWriter(filePath + fileName + nLevel++.ToString() + ".json"))
            {
                outputFile.Write(board.GetBoardState());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedObject == null)
            {
                BoardCell cell = board.GetBoardCell(cursorPos.x, cursorPos.y);
                if (cell.GetState() == BoardCell.BoardCellState.OCUPPIED)
                    selectedObject = cell.GetPlacedObject();
            }
            else
            {
                //Move object to cursor
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (selectedObject != null)
            {
                selectedObject.Rotate(1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            selectedObject = null;
        }
        int nX = Input.GetKeyDown(KeyCode.A) ? -1 : (Input.GetKeyDown(KeyCode.D) ? 1 : 0);
        int nY = Input.GetKeyDown(KeyCode.S) ? -1 : (Input.GetKeyDown(KeyCode.W) ? 1 : 0);
        cursorPos.x = ((cursorPos.x + nX) + columns) % columns;
        cursorPos.y = ((cursorPos.y + nY) + rows) % rows;
        transform.position = new Vector3(cursorPos.x + offset.x, cursorPos.y + offset.y, 0);
    }
}
