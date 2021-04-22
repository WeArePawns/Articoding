using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Base class of all board objects
public class BoardObject : MonoBehaviour
{
    public enum Direction { RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, LEFT, UP_LEFT, UP, UP_RIGHT, PARTIAL };

    static Dictionary<string, int> IDs = new Dictionary<string, int>();
    static int numIDs = 0;

    Direction objectDirection;

    [SerializeField] protected Animator anim;
    protected BoardManager boardManager;
    protected string typeName = "";
    protected string[] argsNames;

    public void SetBoard(BoardManager board)
    {
        boardManager = board;
    }

    public int GetObjectID()
    {
        if (!IDs.ContainsKey(this.GetType().Name))
            IDs[this.GetType().Name] = numIDs++;
        return IDs[this.GetType().Name];
    }

    public string GetName()
    {
        return typeName;
    }

    public string[] GetArgsNames()
    {
        return argsNames;
    }

    public void SetDirection(Direction direction, bool rotate = true)
    {
        objectDirection = direction;

        if (!rotate) return;

        if (direction == Direction.PARTIAL)
            direction = Direction.RIGHT; // Default

        Vector3 lastRot = transform.localEulerAngles;
        lastRot.y = (int)direction * 45.0f;
        transform.localEulerAngles = lastRot;
    }

    public Direction GetDirection()
    {
        return objectDirection;
    }

    public void Rotate(int dir)
    {
        Direction newDir = (Direction)((((int)objectDirection + dir) + 8) % 8);
        SetDirection(newDir);
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    //Virtual method for seralization of additional arguments in child classes
    public virtual string[] GetArgs() { return new string[] { }; }

    //Virtual method for de-seralization of additional arguments in child classes
    public virtual void LoadArgs(string[] args) { }
}
