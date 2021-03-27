using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int rows;
    public int columns;

    public BoardCell cellPrefab;

    private void Awake()
    {
        GenerateBoard();
    }

    // Current gameObject is parent of the cells
    private void GenerateBoard()
    {
        // If a board already exist, destroy it
        DestroyBoard();

        // Initialize board
        for(int y = 0; y < rows; y++)
        {
            for(int x = 0; x < columns; x++)
            {
                BoardCell cell = Instantiate(cellPrefab, transform);
                cell.SetPosition(x, y);
            }
        }
    }

    private void DestroyBoard()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
