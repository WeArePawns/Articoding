using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selectable : MonoBehaviour, IPointerDownHandler
{
    private BoardManager board;
    private BoardObject myBoardObject = null;

    private void Start()
    {
        myBoardObject = GetComponent<BoardObject>();
    }

    public void SetBoard(BoardManager board)
    {
        this.board = board;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        BoardObject boardObject = Instantiate(myBoardObject, transform.position + Vector3.back, Quaternion.identity);
        Destroy(boardObject.gameObject.GetComponent<Selectable>());
        DraggableObject draggable = boardObject.gameObject.AddComponent<DraggableObject>();
        draggable.SetBoard(board);

        draggable.OnPointerDown(eventData);
    }
}
