using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : BoardObject, ILaserEmitter
{
    [Tooltip("Shoot direction")]
    [SerializeField] [Min(0.0f)] private float intensity = 1.0f;

    [SerializeField] private LaserRay laserRayPrefab;
    private LaserRay laserRay;

    public void Start()
    {
        LaserManager.Instance.AddLaserEmitter(this);        
    }

    private void OnDestroy()
    {
        LaserManager.Instance.RemoveLaserEmitter(this);
    }

    public void Emit()
    {
        if (intensity <= 0.0f) return;

        if(laserRay == null)
            laserRay = LaserManager.Instance.CastLaser(transform.position, transform.right, transform);

        foreach (Transform child in laserRay.transform)
            Destroy(child.gameObject);

        laserRay.Cast(transform.position, transform.right, laserRay.transform);
    }

    public void OnLaserEmitted()
    {
        //throw new System.NotImplementedException();
    }

    //TODO:Quitar
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            intensity = 1.0f;
    }
    override
    public string[] GetArgs()
    { 
        return new string[] { intensity.ToString() };
    }

    override
    public void LoadArgs(string[] args)
    {
        if (args.Length > 0)
            intensity = float.Parse(args[0]);
    }

}
