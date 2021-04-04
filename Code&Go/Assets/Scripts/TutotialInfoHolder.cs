using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutotialInfoHolder : MonoBehaviour, IComparable<TutotialInfoHolder>
{
    public int priority;
    public TutorialInfo info;

    private RectTransform mRectTransform;
    private Renderer mRenderer;
    private Collider mCollider;

    private void Awake()
    {
        if ((mRectTransform = GetComponent<RectTransform>()) != null) return;
        if ((mRenderer = GetComponent<Renderer>()) != null) return;
        if ((mCollider = GetComponent<Collider>()) != null) return;
    }

    public Rect GetRect()
    {
        if (mRectTransform != null) return RectUtils.RectTransformToScreenSpace(mRectTransform);
        if (mRenderer != null) return RectUtils.RendererToScreenSpace(mRenderer);
        if (mCollider != null) return RectUtils.ColliderToScreenSpace(mCollider);
        return new Rect(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f, 0.0f); ;
    }

    public int CompareTo(TutotialInfoHolder other)
    {
        return priority - other.priority;
    }
}
