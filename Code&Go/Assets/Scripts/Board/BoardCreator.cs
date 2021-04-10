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

    [SerializeField] Toggle limits;

    [SerializeField] GameObject indicatorPrefab;

    [SerializeField] private Vector2 boardInitOffsetLeftDown;
    [SerializeField] private Vector2 boardInitOffsetRightUp;
    private bool buildLimits = false;
    [SerializeField] private Camera mainCamera;

    private string fileName = "level";
    private int nLevel = 1;
    private string filePath = "";

    private Vector2Int cursorPos = Vector2Int.zero, selectedPos = -Vector2Int.one;
    private Vector2 offset = new Vector2(-0.3f, 0.3f);

    private GameObject selectedIndicator = null;

    private int rows, columns;

    private void Start()
    {
        GenerateNewBoard();
        filePath = Application.dataPath + "/LevelsCreated/";
        rows = board.GetRows();
        columns = board.GetColumns();
        material.color = Color.yellow;
        FitBoard();
    }

    private void FitBoard()
    {
        if (mainCamera != null)
        {
            float height = mainCamera.orthographicSize * 2, width = height * mainCamera.aspect;
            float xPos = Mathf.Lerp(-width / 2.0f, width / 2.0f, boardInitOffsetLeftDown.x);
            float yPos = Mathf.Lerp(-height / 2.0f, height / 2.0f, boardInitOffsetLeftDown.y);
            height *= (1.0f - (boardInitOffsetLeftDown.y + boardInitOffsetRightUp.y));
            width *= (1.0f - (boardInitOffsetLeftDown.x + boardInitOffsetRightUp.x));

            int limits = (buildLimits) ? 2 : 0;
            float boardHeight = (float)board.GetRows() + limits, boardWidth = (float)board.GetColumns() + limits;
            float xRatio = width / boardWidth, yRatio = height / boardHeight;
            float ratio = Mathf.Min(xRatio, yRatio);
            float offsetX = (-boardWidth * ratio) / 2.0f + (limits / 2.0f + 0.5f) * ratio, offsetY = (-boardHeight * ratio) / 2.0f + (limits / 2.0f + 0.5f) * ratio;

            //Fit the board on the screen and resize it
            board.transform.position = new Vector3(xPos + width / 2.0f + offsetX, 0, yPos + height / 2.0f + offsetY);
            board.transform.localScale = new Vector3(ratio, 1.0f, ratio);
        }
    }

    private void Update()
    {
        ManageInput();

        //CursorMovement
        int nX = Input.GetKeyDown(KeyCode.A) ? -1 : (Input.GetKeyDown(KeyCode.D) ? 1 : 0);
        int nY = Input.GetKeyDown(KeyCode.S) ? -1 : (Input.GetKeyDown(KeyCode.W) ? 1 : 0);
        cursorPos.x = ((cursorPos.x + nX) + columns) % columns;
        cursorPos.y = ((cursorPos.y + nY) + rows) % rows;
        transform.localPosition = new Vector3(cursorPos.x + offset.x, 0, cursorPos.y + offset.y);
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
                    selectedIndicator.transform.localPosition = new Vector3(cursorPos.x + offset.x, cursorPos.y + offset.y, 0);
                }
                //Move object to cursor
                else if (selectedPos != -Vector2Int.one)
                {
                    board.MoveBoardObject(selectedPos, cursorPos);
                    //board.MoveObject(selectedPos, cursorPos - selectedPos, 0.5f);
                    selectedPos = cursorPos;
                    if (selectedIndicator != null)
                        selectedIndicator.transform.localPosition = new Vector3(cursorPos.x + offset.x, cursorPos.y + offset.y, 0);
                }
            }
        }
        //Rotate the selectedObject E clockwise Q anti-clockwise
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedPos != -Vector2Int.one)
            {
                board.RotateBoardObject(selectedPos, Input.GetKeyDown(KeyCode.E) ? 1 : -1);
                //board.RotateObject(selectedPos, Input.GetKeyDown(KeyCode.E) ? 1 : -1, 0.5f);
            }
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
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AddObject(4);
        }
        else if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            ReplaceCell(0, cursorPos.x, cursorPos.y);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ReplaceCell(1, cursorPos.x, cursorPos.y);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            ReplaceCell(2, cursorPos.x, cursorPos.y);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            ReplaceCell(3, cursorPos.x, cursorPos.y);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            ReplaceCell(4, cursorPos.x, cursorPos.y);
        }
    }

    public void GenerateNewBoard()
    {
        int columns = int.Parse(columnsField.text), rows = int.Parse(rowsField.text);
        if (columns <= 0 || rows <= 0) return;
        buildLimits = limits.isOn;

        board.SetColumns(columns);
        board.SetRows(rows);
        board.transform.localScale = Vector3.one;
        board.transform.localPosition = Vector3.zero;
        board.GenerateBoard();
        if (buildLimits) board.GenerateLimits();

        DeselectObject();
        cursorPos = Vector2Int.zero;
        transform.localPosition = Vector3.zero;
        this.rows = rows;
        this.columns = columns;
        FitBoard();
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

    private void ReplaceCell(int id, int x, int y)
    {
        board.ReplaceCell(id, x, y);
    }
}
