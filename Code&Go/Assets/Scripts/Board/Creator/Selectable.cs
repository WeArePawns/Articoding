using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AssetPackage;
using Simva.Api;

public class Selectable : MonoBehaviour, IMouseListener
{
    private BoardManager board;
    private BoardObject myBoardObject = null;
    private ArgumentLoader argumentLoader = null;

    private void Start()
    {
        myBoardObject = GetComponent<BoardObject>();
    }

    public void SetBoard(BoardManager board)
    {
        this.board = board;
    }

    public void SetArgumentLoader(ArgumentLoader loader)
    {
        argumentLoader = loader;
    }

    public void OnMouseButtonDown(int index)
    {
        if (index != 0) return;
        if (!board.GetOrbitCamera().IsReset()) return;

        BoardObject boardObject = Instantiate(myBoardObject, transform.position, Quaternion.identity);
        boardObject.transform.localScale = transform.lossyScale;
        Destroy(boardObject.gameObject.GetComponent<Selectable>());
        DraggableObject draggable = boardObject.gameObject.AddComponent<DraggableObject>();
        draggable.SetBoard(board);
        draggable.SetArgumentLoader(argumentLoader);
        draggable.SetCameraInput(board.GetMouseInput());
        draggable.SetOrbitCamera(board.GetOrbitCamera());

        string name = boardObject.GetName();
        TrackerAsset.Instance.setVar("element_type", name.ToLower());
        TrackerAsset.Instance.setVar("element_name", boardObject.GetNameWithIndex().ToLower());
        TrackerAsset.Instance.setVar("action", "create");
        TrackerAsset.Instance.GameObject.Interacted(boardObject.GetID());

        draggable.OnMouseButtonDown(index);
    }

    public void OnMouseButtonUp(int index)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMouseButtonUpAnywhere(int index)
    {
        //throw new System.NotImplementedException();
    }
}
