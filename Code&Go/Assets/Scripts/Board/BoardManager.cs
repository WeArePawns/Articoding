using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int columns;

    [SerializeField] private BoardCell cellPrefab;
    [SerializeField] private Transform cellsParent;
    [SerializeField] private Transform elementsParent;
    [SerializeField] private BoardObject[] elements;


    // Hidden atributtes
    private BoardCell[,] board;


    private void Awake()
    {
        GenerateBoard();
        GenerateBoardElements();
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
            for(int x = 0; x < columns; x++)
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
    
    private void GenerateBoardElements()
    {
        // Read from file or whatever

        // TODO: delete this
        BoardObject bObject = Instantiate(elements[2], elementsParent);
        bObject.SetBoard(this);
        board[0, 2].PlaceObject(bObject);
    }

}
