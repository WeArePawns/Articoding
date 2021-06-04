using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using AssetPackage;

public class ModifiableBoardCell : MonoBehaviour, IMouseListener
{
    private BoardCell cell;
    private BoardManager boardManager;
    private bool modifiable = true;

    private OrbitCamera orbitCamera;
    private CameraMouseInput cameraInput;

    private void Start()
    {
        cell = GetComponent<BoardCell>();
    }

    public void SetBoardManager(BoardManager board)
    {
        boardManager = board;
        orbitCamera = boardManager.GetOrbitCamera();
        cameraInput = boardManager.GetMouseInput();
    }

    public void OnMouseButtonDown(int index)
    {
        //Change the type of the cell
        if (index == 0 && modifiable && cell.GetPlacedObject() == null && orbitCamera.IsReset())
        {
            Vector2Int pos = cell.GetPosition();
            boardManager.ReplaceCell(cell.GetNextID(), pos.x, pos.y);
            if (cameraInput != null) cameraInput.SetDragging(true);
            TrackerAsset.Instance.setVar("cell_pos", pos.ToString());
            TrackerAsset.Instance.setVar("new_cell_type", cell.GetNextID());
            TrackerAsset.Instance.GameObject.Used("board_cell_change");
        }
    }

    public void OnMouseButtonUpAnywhere(int index)
    {
        if (cameraInput != null) cameraInput.SetDragging(false);
    }

    public void SetModifiable(bool modifiable)
    {
        this.modifiable = modifiable;
    }

    public void OnMouseButtonUp(int index)
    {
        //throw new System.NotImplementedException();
    }
}
