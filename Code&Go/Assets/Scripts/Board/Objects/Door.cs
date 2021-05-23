using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BoardObject
{
    private bool open = false;
    [SerializeField] GameObject objectCollider;
    // DO STUFF I GUESS
    /*private void Awake()
    {
        typeName = "Puerta_";
        argsNames = new string[1] { "Abierta" };
    }*/

    override public string[] GetArgs()
    {
        return new string[] { open.ToString() };
    }

    override public void LoadArgs(string[] args)
    {
        if (args != null && args.Length > 0)
            try
            {
                SetActive(bool.Parse(args[0]));
            }catch
            {
                try
                {
                    SetActive(int.Parse(args[0]) > 0);
                }
                catch
                {
                    Debug.Log("Parametro no valido");
                }
            }
    }

    public void SetActive(bool open)
    {
        if (this.open == open) return;
        //Animacion
        Invoke("DeactivateCollider", 0.01f);
        this.open = open;
    }

    public void DeactivateCollider()
    {
        objectCollider.SetActive(!open);
    }
}
