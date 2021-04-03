using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRay : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [Space]
    [SerializeField] private float rayWidth;
    //[SerializeField] private Material rayMaterial;
    [SerializeField] private Color color;

    private void Awake()
    {
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.material.color = color;
    }

    public void Cast(Vector3 from, Vector3 direction, Transform parent)
    {
        List<Vector3> positions = new List<Vector3>();
        ILaserReflector reflector = null;
        ILaserReceiver receiver = null;

        positions.Add(from);
        RaycastHit hitInfo;
        int layerMask = 1 << LayerMask.NameToLayer("LaserLayer");
        if (Physics.Raycast(new Ray(from, direction), out hitInfo, 20.0f, layerMask))
        {
            positions.Add(hitInfo.point);
            // Collider is a child of main object
            reflector = hitInfo.transform.parent.GetComponent<ILaserReflector>();
            receiver = hitInfo.transform.parent.GetComponent<ILaserReceiver>();
        }
        else
        {
            positions.Add(from + direction * 20.0f); // This should not happen
        }

        lineRenderer.SetPositions(positions.ToArray());


        if (reflector != null)
        {
            Vector3[] refletedFrom = null;
            Vector3[] reflectedDirection = null;
            reflector.Reflect(hitInfo.point, direction, hitInfo.normal, out refletedFrom, out reflectedDirection);
            if (refletedFrom != null)
            {
                for (int i = 0; i < refletedFrom.Length; i++)
                {
                    LaserManager.Instance.CastLaser(refletedFrom[i], reflectedDirection[i], parent);
                }
            }
        }

        if(receiver != null)
        {
            LaserManager.Instance.AddLaserReceiver(receiver);
        }
    }
}
