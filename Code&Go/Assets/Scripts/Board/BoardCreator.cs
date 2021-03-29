using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] BoardManager board;
    [SerializeField] Material material;

    private string fileName = "level";
    private int nLevel = 1;
    private string filePath = "";

    private Vector2Int cursorPos = Vector2Int.zero, selectedPos = Vector2Int.zero;
    private Vector2 offset = new Vector2(-0.3f, 0.3f);
    private BoardObject selectedObject = null;

    int rows, columns;

    private void Start()
    {
        filePath = Application.dataPath + "/LevelsCreated/";
        rows = board.GetRows();
        columns = board.GetColumns();
        material.color = Color.yellow;
    }

    void Update()
    {
        ManageInput();

        //CursorMovement
        int nX = Input.GetKeyDown(KeyCode.A) ? -1 : (Input.GetKeyDown(KeyCode.D) ? 1 : 0);
        int nY = Input.GetKeyDown(KeyCode.S) ? -1 : (Input.GetKeyDown(KeyCode.W) ? 1 : 0);
        cursorPos.x = ((cursorPos.x + nX) + columns) % columns;
        cursorPos.y = ((cursorPos.y + nY) + rows) % rows;
        transform.position = new Vector3(cursorPos.x + offset.x, cursorPos.y + offset.y, 0);
    }

    private void AddObject(int id)
    {
        if (selectedObject != null) return;

        BoardCell cell = board.GetBoardCell(cursorPos.x, cursorPos.y);
        //If the cursor is in an empty position
        if (cell != null && cell.GetState() != BoardCell.BoardCellState.OCUPPIED)
            board.AddBoardObject(id, cursorPos.x, cursorPos.y);
    }

    private void DeselectObject()
    {
        selectedObject = null;
        selectedPos = new Vector2Int(-1, -1);
        material.color = Color.yellow;
    }

    private void ManageInput()
    {
        //Save the current board
        if (Input.GetKeyDown(KeyCode.G))
        {
            using (StreamWriter outputFile = new StreamWriter(filePath + fileName + nLevel++.ToString() + ".json"))
            {
                outputFile.Write(board.GetBoardState());
            }
            Console.WriteLine("Archivo Guardado");
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            BoardCell cell = board.GetBoardCell(cursorPos.x, cursorPos.y);
            if (cell != null)
            {
                //Select a new object
                if (selectedObject == null && cell.GetState() == BoardCell.BoardCellState.OCUPPIED)
                {
                    selectedObject = cell.GetPlacedObject();
                    selectedPos = cursorPos;
                    material.color = Color.blue;
                }
                //Move object to cursor
                else if (selectedObject != null && cell.GetState() == BoardCell.BoardCellState.FREE)
                {
                    board.MoveBoardObject(selectedPos, cursorPos);
                    selectedPos = cursorPos;
                }
            }
        }
        //Rotate the selectedObject E clockwise Q anti-clockwise
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedObject != null)
                selectedObject.Rotate(Input.GetKeyDown(KeyCode.E) ? 1 : -1);
        }

        //Deselect the current object
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeselectObject();
        }
        //Delete the selected object
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (selectedObject != null)
            {
                board.RemoveBoardObject(selectedPos.x, selectedPos.y);
                DeselectObject();
            }
        }
        //Create new elements
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddObject(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddObject(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddObject(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddObject(3);
        }
    }
}
