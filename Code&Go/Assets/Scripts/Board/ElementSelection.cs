using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSelection : MonoBehaviour
{
    [SerializeField] private BoardCell cellPrefab;
    [SerializeField] private Transform cellsParent;
    [SerializeField] private Transform elementsParent;
    [SerializeField] private BoardObject[] elements;
    [SerializeField] private BoardManager board;
    [SerializeField] private ArgumentLoader loader;

    private int rows = 1;
    private int columns = 1;

    public void GenerateSelector()
    {
        DestroySelector();

        rows = Mathf.Min(board.GetRows(), elements.Length);
        columns = Mathf.CeilToInt(elements.Length / (float)rows);
        int elementIndex = 0;
        columns = Mathf.Clamp(columns, 1, int.MaxValue);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                BoardCell cell = Instantiate(cellPrefab, cellsParent);
                cell.SetPosition(x, y);
                if (elementIndex < elements.Length)
                {
                    BoardObject boardObject = Instantiate(elements[elementIndex++], elementsParent);
                    Selectable selectable = boardObject.gameObject.AddComponent<Selectable>();
                    selectable.SetBoard(board);
                    selectable.SetArgumentLoader(loader);
                    cell.PlaceObject(boardObject);
                }
            }
        }
    }

    public void DestroySelector()
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

    public int GetColumns()
    {
        return columns;
    }
    public int GetRows()
    {
        return rows;
    }
}
