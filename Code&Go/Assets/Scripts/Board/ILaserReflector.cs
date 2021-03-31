using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILaserReflector
{
    void Reflect(Vector3 inFrom, Vector3 inDirection, Vector3 inNormal, out Vector3[] outFrom, out Vector3[] outDirection);
}
