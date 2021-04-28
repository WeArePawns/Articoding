using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        this.argumentLoader = loader;
    }

    public void OnMouseButtonDown(int index)
    {
        if (index != 0) return;

        BoardObject boardObject = Instantiate(myBoardObject, transform.position, Quaternion.identity);
        boardObject.transform.localScale = transform.lossyScale;
        Destroy(boardObject.gameObject.GetComponent<Selectable>());
        DraggableObject draggable = boardObject.gameObject.AddComponent<DraggableObject>();
        draggable.SetBoard(board);
        draggable.SetArgumentLoader(argumentLoader);

        draggable.OnMouseButtonDown(index);
    }

    public void OnMouseButtonUp(int index)
    {
        //throw new System.NotImplementedException();
    }
}
