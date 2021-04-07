using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMirror : BoardObject, ILaserReflector
{
    [SerializeField] private Collider reflectCollider;

    private void Awake()
    {
        typeName = "Espejo";
    }

    public void Reflect(Vector3 inFrom, Vector3 inDirection, Vector3 inNormal, out Vector3[] outFrom, out Vector3[] outDirection)
    {
        if (Vector3.Dot(inFrom, reflectCollider.gameObject.transform.right.normalized) <= 0.0) 
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
