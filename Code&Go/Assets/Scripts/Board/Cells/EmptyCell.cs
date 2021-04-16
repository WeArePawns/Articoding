using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyCell : BoardCell
{
    public override void OnObjectPlaced()
    {
        Rigidbody rb = placedObject.gameObject.AddComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;
        // Do stuff
        // Mandar señal de GameOver o lo que sea
        //boardManager.DoStuff();
    }

    public override string[] GetArgs()
    {
        return new string[0];
    }
}
