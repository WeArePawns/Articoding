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
    protected int index = 1;
    protected string[] argsNames;

    public virtual void SetBoard(BoardManager board)
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

    public string GetNameAsLower()
    {
        return typeName.ToLower();
    }

    public string GetNameWithIndex()
    {
        return typeName + " " + index.ToString();
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

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }

    //Virtual method for seralization of additional arguments in child classes
    public virtual string[] GetArgs() { return new string[] { }; }

    //Virtual method for de-seralization of additional arguments in child classes
    public virtual void LoadArgs(string[] args) { }
}
