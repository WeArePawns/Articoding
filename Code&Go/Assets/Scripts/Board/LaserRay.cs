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

    public void Cast(Vector3 from, Vector3 direction)
    {
        List<Vector3> positions = new List<Vector3>();

        positions.Add(from);
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(from, direction), out hitInfo))
        {
            positions.Add(hitInfo.point);
        }
        else
        {
            positions.Add(from + direction * 20.0f); // This should not happen
        }

        lineRenderer.SetPositions(positions.ToArray());
    }
}
