using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : BoardObject, ILaserEmitter
{
    [Tooltip("Shoot direction")]
    [SerializeField] [Min(0.0f)] private float intensity;

    [SerializeField] private LaserRay laserRayPrefab;
    private LaserRay laserRay;

    public void Start()
    {
        LaserManager.Instance.AddLaserEmitter(this);
        laserRay = LaserManager.Instance.CastLaser(transform.position, transform.right, transform);
    }

    private void OnDestroy()
    {
        LaserManager.Instance.RemoveLaserEmitter(this);
    }

    public void Emit()
    {
        foreach (Transform child in laserRay.transform)
            Destroy(child.gameObject);

        laserRay.Cast(transform.position, transform.right, laserRay.transform);
    }

    public void OnLaserEmitted()
    {
        //throw new System.NotImplementedException();
    }
}
