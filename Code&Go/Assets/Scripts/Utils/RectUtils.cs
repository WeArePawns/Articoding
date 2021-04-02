using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectUtils : MonoBehaviour
{
    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * transform.pivot), size);
    }
}
