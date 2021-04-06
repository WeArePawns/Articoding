using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEmitter : BoardObject, ILaserEmitter
{
    [Tooltip("Shoot direction")]
    [SerializeField] [Min(0.0f)] private float intensity = 1.0f;

    [SerializeField] private LaserRay laserRayPrefab;
    private LaserRay laserRay;

    [SerializeField] private ParticleSystem onParticles;

    private void Awake()
    {
        typeName = "Laser";
    }

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
        if (intensity <= 0.0f)
        {
            onParticles.Stop();
            return;
        }

        onParticles.Play();

        if (laserRay == null)
            laserRay = LaserManager.Instance.CastLaser(transform.position, transform.right, transform);

        foreach (Transform child in laserRay.transform)
            Destroy(child.gameObject);

        laserRay.Cast(transform.position, transform.right, laserRay.transform);
    }

    public void OnLaserEmitted()
    {
        //throw new System.NotImplementedException();
    }

    public void ChangeIntensity(float newIntensity)
    {
        if (newIntensity > 0.0f)
            intensity = newIntensity;
    }

    override public string[] GetArgs()
    {
        return new string[] { intensity.ToString() };
    }

    override public void LoadArgs(string[] args)
    {
        if (args != null && args.Length > 0)
            intensity = float.Parse(args[0]);
    }

}
