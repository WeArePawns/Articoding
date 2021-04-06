using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BoardObject
{
    private bool open = false;
    [SerializeField] Collider objectCollider;
    // DO STUFF I GUESS
    private void Awake()
    {
        typeName = "Door";
    }

    override public string[] GetArgs()
    {
        return new string[] { open.ToString() };
    }

    override public void LoadArgs(string[] args)
    {
        if (args != null && args.Length > 0)
            SetActive(bool.Parse(args[0]));
    }

    public void SetActive(bool open)
    {
        if (this.open == open) return;
        //Animacion
        Invoke("DeactivateCollider", 0.5f);
        this.open = open;
    }

    public void DeactivateCollider()
    {
        objectCollider.enabled = !open;
    }
}
