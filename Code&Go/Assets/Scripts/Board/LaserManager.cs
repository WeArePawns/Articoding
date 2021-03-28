﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    public static LaserManager Instance;

    [SerializeField] private LaserRay laserRayPrefab;
    [SerializeField] private Transform laserParent;

    private List<LaserRay> laserRays;

    private List<ILaserEmitter> emitters;
    private List<ILaserReceiver> receivers;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
        emitters = new List<ILaserEmitter>();
        receivers = new List<ILaserReceiver>();
    }

    private void Update()
    {
        foreach(ILaserEmitter emitter in emitters)
        {
            emitter.Emit();
            // TODO: expandir si es necesario
        }
    }

    public LaserRay CastLaser(Vector3 from, Vector3 direction)
    {
        LaserRay laser = Instantiate(laserRayPrefab, laserParent);
        laser.Cast(from, direction, laserParent);
        return laser;
    }

    public LaserRay CastLaser(Vector3 from, Vector3 direction, Transform parent)
    {
        if (parent == null) parent = laserParent;

        LaserRay laser = Instantiate(laserRayPrefab, parent.transform);
        laser.Cast(from, direction, parent);
        return laser;
    }

    public void AddLaserEmitter(ILaserEmitter emitter)
    {
        if (emitters.Contains(emitter)) return;
        emitters.Add(emitter);
    }

    public void RemoveLaserEmitter(ILaserEmitter emitter)
    {
        emitters.Remove(emitter);
    }

    public void AddLaserReceiver(ILaserReceiver receiver)
    {
        if (receivers.Contains(receiver)) return;
        receivers.Add(receiver);
    }

    public void RemoveLaserReceiver(ILaserReceiver receiver)
    {
        receivers.Remove(receiver);
    }
}
