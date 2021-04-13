using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ModifiableBoardCell : MonoBehaviour, IMouseListener
{
    private BoardCell cell;

    private void Start()
    {
        cell = GetComponent<BoardCell>();
    }

    public void OnMouseButtonDown(int index)
    {
        //Change the type of the cell
        if (index == 0)
            print("Cell clicked\n");
    }

    public void OnMouseButtonUp(int index)
    {
        //throw new System.NotImplementedException();
    }
}
