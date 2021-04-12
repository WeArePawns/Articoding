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

    private int rows = 1;
    private int columns = 1;

    public void GenerateSelector()
    {
        rows = board.GetRows();
        columns = elements.Length / rows;
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
                    cell.PlaceObject(boardObject);
                }
            }
        }
    }

    public int GetColumns()
    {
        return columns;
    }
}
