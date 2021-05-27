using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssetPackage;

public class BoardCreator : MonoBehaviour
{
    [SerializeField] BoardManager board;
    [SerializeField] ElementSelection elementSelection;
    [SerializeField] Material material;

    [SerializeField] InputField columnsField;
    [SerializeField] InputField rowsField;
    [SerializeField] InputField saveFileNameField;

    [SerializeField] ArgumentLoader argumentLoader;

    [SerializeField] InputField loadFileNameField;

    [SerializeField] CameraFit cameraFit;

    [SerializeField] GameObject indicatorPrefab;

    [SerializeField] private bool keyBoardControls = false;
    [SerializeField] private int maxSize = 15;

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
        board.SetArgLoader(argumentLoader);
        material.color = Color.yellow;
        GetComponent<MeshRenderer>().enabled = keyBoardControls;
        columnsField.onValueChanged.AddListener(delegate { CheckInputField(columnsField); });
        rowsField.onValueChanged.AddListener(delegate { CheckInputField(rowsField); });
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!keyBoardControls) return;
        ManageInput();

        //CursorMovement
        int nX = Input.GetKeyDown(KeyCode.A) ? -1 : (Input.GetKeyDown(KeyCode.D) ? 1 : 0);
        int nY = Input.GetKeyDown(KeyCode.S) ? -1 : (Input.GetKeyDown(KeyCode.W) ? 1 : 0);
        cursorPos.x = ((cursorPos.x + nX) + columns) % columns;
        cursorPos.y = ((cursorPos.y + nY) + rows) % rows;
        transform.localPosition = new Vector3(cursorPos.x + offset.x, 0, cursorPos.y + offset.y);
    }
#endif

    private void CheckInputField(InputField field)
    {
        if (field.text.Length == 0) return;

            int value = int.Parse(field.text);
        if (value <= 0)
            field.text = "1";
        else if (value > maxSize)
            field.text = maxSize.ToString();
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
            else if (board.HasHint(cursorPos))
                board.DeleteHint(cursorPos, 0);
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
        //Create new cell types
        else if (Input.GetKeyDown(KeyCode.Keypad1))
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
        //Add hints
        else if (Input.GetKeyDown(KeyCode.J))
        {
            board.AddBoardHint(cursorPos, 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            board.AddBoardHint(cursorPos, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            board.AddBoardHint(cursorPos, 1, 2);
        }
        //Rotate Hints
        else if (Input.GetKeyDown(KeyCode.U))
        {
            board.RotateHint(cursorPos, 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            board.RotateHint(cursorPos, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            board.RotateHint(cursorPos, 1, 2);
        }
    }

    private void ResetCursor()
    {
        //Reset the cursor
        DeselectObject();
        cursorPos = Vector2Int.zero;
        transform.localPosition = Vector3.zero;
        this.rows = board.GetRows();
        this.columns = board.GetColumns();
    }

    public void GenerateNewBoard()
    {
        int columns = int.Parse(columnsField.text), rows = int.Parse(rowsField.text);
        if (columns <= 0 || rows <= 0) return;

        board.SetColumns(columns);
        board.SetRows(rows);

        //Create an empty board
        board.GenerateBoard();
        board.GenerateHoles();
        board.GenerateLimits();

        //Initialize the element selection section
        elementSelection.GenerateSelector();

        ResetCursor();

        FitBoard();

        TrackerAsset.Instance.setVar("rows", rows);
        TrackerAsset.Instance.setVar("columns", columns);
        TrackerAsset.Instance.GameObject.Used("board_created");
    }

    public void FitBoard()
    {
        board.SetFocusPointOffset(new Vector3((board.GetColumns() - 2) / 2.0f + 0.5f, 0.0f, ((board.GetRows() - 2) / 2.0f + 0.5f) - 1));
        cameraFit.FitBoard(board.GetRows() + 1 + elementSelection.GetRows(), Mathf.Max(board.GetColumns(), elementSelection.GetColumns()));
    }

    public void SaveBoard()
    {
        fileName = saveFileNameField.text;
        if (fileName == "") fileName = "level";
        using (StreamWriter outputFile = new StreamWriter(filePath + fileName + nLevel++.ToString() + ".json"))
        {
            outputFile.Write(board.GetBoardStateAsString());
        }
        Console.WriteLine("Archivo Guardado");
    }

    public void LoadBoard()
    {
        fileName = Application.dataPath + "/" + loadFileNameField.text;
        using (StreamReader inputFile = new StreamReader(fileName))
        {
            BoardState state = BoardState.FromJson(inputFile.ReadToEnd());
            board.LoadBoard(state, false);

            elementSelection.GenerateSelector();

            ResetCursor();

            FitBoard();
        }
        Console.WriteLine("Archivo Cargado");
    }

    private void ReplaceCell(int id, int x, int y)
    {
        board.ReplaceCell(id, x, y);
    }
}
