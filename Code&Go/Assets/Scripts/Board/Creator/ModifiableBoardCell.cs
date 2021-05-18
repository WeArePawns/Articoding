﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ModifiableBoardCell : MonoBehaviour, IMouseListener
{
    private BoardCell cell;
    private BoardManager boardManager;
    private bool modifiable = true;

    private void Start()
    {
        cell = GetComponent<BoardCell>();
    }

    public void SetBoardManager(BoardManager board)
    {
        boardManager = board;
    }

    public void OnMouseButtonDown(int index)
    {
        //Change the type of the cell
        if (index == 0 && modifiable && cell.GetPlacedObject() == null)
        {
            Vector2Int pos = cell.GetPosition();
            boardManager.ReplaceCell(cell.GetNextID(), pos.x, pos.y);
        }
    }

    public void OnMouseButtonUp(int index)
    {
        //throw new System.NotImplementedException();
    }

    public void SetModifiable(bool modifiable)
    {
        this.modifiable = modifiable;
    }
}
