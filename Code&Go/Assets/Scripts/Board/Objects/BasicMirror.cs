using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMirror : BoardObject, ILaserReflector
{
    [SerializeField] private Collider reflectCollider;

    private void Awake()
    {
        typeName = "Espejo";
        argsNames = new string[0];
    }

    public void Reflect(Vector3 inFrom, Vector3 inDirection, Vector3 inNormal, out Vector3[] outFrom, out Vector3[] outDirection)
    {
        float dot = Vector3.Dot(inDirection, reflectCollider.gameObject.transform.up);
        if (dot <= 0.0f) 
        {
            outFrom = new Vector3[0];
            outDirection = new Vector3[0];
            return; 
        }

        outFrom = new Vector3[1];
        outDirection = new Vector3[1];

        outFrom[0] = inFrom;
        outDirection[0] = Vector3.Reflect(inDirection, inNormal);
    }
}
