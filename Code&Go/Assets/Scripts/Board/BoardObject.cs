using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class of all board objects
public class BoardObject : MonoBehaviour
{
    // Needed for serialization ????
    int objectID;

    BoardManager boardManager;

    public void SetBoard(BoardManager board)
    {
        boardManager = board;
    }
}
