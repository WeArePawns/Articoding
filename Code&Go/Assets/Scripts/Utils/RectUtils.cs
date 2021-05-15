using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RectUtils : MonoBehaviour
{
    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * transform.pivot), size);
    }

    // Based on https://stackoverflow.com/questions/65417634/draw-bounding-rectangle-screen-space-around-a-game-object-with-a-renderer-wor
    public static Rect ColliderToScreenSpace(Collider collider, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;

        //Sync collider position with transform
        Physics.SyncTransforms();

        Vector3 c = collider.bounds.center;
        Vector3 e = collider.bounds.extents;

        Vector3[] worldCorners = new[] {
            new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
        };

        IEnumerable<Vector3> screenCorners = worldCorners.Select(corner => camera.WorldToScreenPoint(corner));
        float maxX = screenCorners.Max(corner => corner.x);
        float minX = screenCorners.Min(corner => corner.x);
        float maxY = screenCorners.Max(corner => corner.y);
        float minY = screenCorners.Min(corner => corner.y);

        Vector2 size = new Vector2(maxX - minX, maxY - minY);
        return new Rect((Vector2)camera.WorldToScreenPoint(c) - size / 2.0f, size);
    }

    public static Rect RendererToScreenSpace(Renderer renderer, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;

        Vector3 c = renderer.bounds.center;
        Vector3 e = renderer.bounds.extents;

        Vector3[] worldCorners = new[] {
            new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
            new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
        };

        IEnumerable<Vector3> screenCorners = worldCorners.Select(corner => camera.WorldToScreenPoint(corner));
        float maxX = screenCorners.Max(corner => corner.x);
        float minX = screenCorners.Min(corner => corner.x);
        float maxY = screenCorners.Max(corner => corner.y);
        float minY = screenCorners.Min(corner => corner.y);

        Vector2 size = new Vector2(maxX - minX, maxY - minY);
        return new Rect((Vector2)camera.WorldToScreenPoint(c) - size / 2.0f, size);
    }
}
