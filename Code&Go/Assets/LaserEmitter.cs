using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : BoardObject, ILaserEmitter
{
    [Tooltip("Shoot direction")]
    [SerializeField] private Vector2 direction;

    public void Emit()
    {
        throw new System.NotImplementedException();
    }

    public void OnLaserEmitted()
    {
        throw new System.NotImplementedException();
    }
}
