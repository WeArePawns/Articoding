using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    public static LaserManager Instance;

    [SerializeField] private LaserRay laserRayPrefab;
    [SerializeField] private Transform laserParent;

    private List<LaserRay> laserRays;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }

    public void CastLaser(Vector3 from, Vector3 direction)
    {
        LaserRay laser = Instantiate(laserRayPrefab, laserParent);
        laser.Cast(from, direction);
    }

    private void Update()
    {
        // TODO: eliminar esto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CastLaser(Vector3.right * -2 + Vector3.forward * -0.2f, Vector3.up + Vector3.right);
        }
    }
}
