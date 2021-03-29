using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] BoardManager board;
    [SerializeField] Material material;

    [SerializeField] InputField columnsField;
    [SerializeField] InputField rowsField;
    [SerializeField] InputField fileNameField;

    [SerializeField] GameObject indicatorPrefab;

    private string fileName = "level";
    private int nLevel = 1;
    private string filePath = "";

    private Vector2Int cursorPos = Vector2Int.zero, selectedPos = -Vector2Int.one;
    private Vector2 offset = new Vector2(-0.3f, 0.3f);

    private GameObject selectedIndicator = null;

    private int rows, columns;

    private void Start()
    {
        filePath = Application.dataPath + "/LevelsCreated/";
        rows = board.GetRows();
        columns = board.GetColumns();
        material.color = Color.yellow;
    }

    private void Update()
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
        if (selectedPos != -Vector2Int.one) return;

        BoardCell cell = board.GetBoardCell(cursorPos.x, cursorPos.y);
        //If the cursor is in an empty position
        if (cell != null && cell.GetState() != BoardCell.BoardCellState.OCUPPIED)
            board.AddBoardObject(id, cursorPos.x, cursorPos.y);
    }

    private void DeselectObject()
    {
        selectedPos = -Vector2Int.one;
        material.color = Color.yellow;
        if (selectedIndicator != null)
            Destroy(selectedIndicator);
    }

    private void ManageInput()
    {
        //Save the current board
        if (Input.GetKeyDown(KeyCode.G))
        {
            SaveBoard();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            BoardCell cell = board.GetBoardCell(cursorPos.x, cursorPos.y);
            if (cell != null)
            {
                //Select a new object
                if (selectedPos == -Vector2Int.one && cell.GetState() == BoardCell.BoardCellState.OCUPPIED)
                {
                    selectedPos = cursorPos;
                    material.color = Color.blue;

                    selectedIndicator = Instantiate(indicatorPrefab, transform.parent);
                    selectedIndicator.transform.position = new Vector3(cursorPos.x + offset.x, cursorPos.y + offset.y, 0);
                }
                //Move object to cursor
                else if (selectedPos != -Vector2Int.one && cell.GetState() == BoardCell.BoardCellState.FREE)
                {
                    board.MoveBoardObject(selectedPos, cursorPos);
                    selectedPos = cursorPos;
                    if (selectedIndicator != null)
                        selectedIndicator.transform.position = new Vector3(cursorPos.x + offset.x, cursorPos.y + offset.y, 0);
                }
            }
        }
        //Rotate the selectedObject E clockwise Q anti-clockwise
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedPos != -Vector2Int.one)
                board.RotateBoardObject(selectedPos, Input.GetKeyDown(KeyCode.E) ? 1 : -1);
        }

        //Deselect the current object
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeselectObject();
        }
        //Delete the selected object
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (selectedPos != -Vector2Int.one)
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

    public void GenerateNewBoard()
    {
        int columns = int.Parse(columnsField.text), rows = int.Parse(rowsField.text);
        if (columns <= 0 || rows <= 0) return;

        board.SetColumns(columns);
        board.SetRows(rows);
        board.GenerateBoard();

        DeselectObject();
        cursorPos = Vector2Int.zero;
        transform.position = Vector3.zero;
        this.rows = rows;
        this.columns = columns;
    }
    public void SaveBoard()
    {
        fileName = fileNameField.text;
        if (fileName == "") fileName = "level";
        using (StreamWriter outputFile = new StreamWriter(filePath + fileName + nLevel++.ToString() + ".json"))
        {
            outputFile.Write(board.GetBoardState());
        }
        Console.WriteLine("Archivo Guardado");
    }
}
