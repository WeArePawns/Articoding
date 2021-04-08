using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColoredCell : BoardCell
{
    [SerializeField] private Color color;
    [SerializeField] private Material cellMaterial;

    [SerializeField] private Renderer mRenderer;
    private Material mMaterial;

#if UNITY_EDITOR

    private void Start()
    {
        if (mMaterial == null)
            mMaterial = new Material(cellMaterial);
        mMaterial.color = color;
        mRenderer.material = mMaterial;
    }

    private void Update()
    {
        if (mMaterial == null)
            mMaterial = new Material(cellMaterial);
        mMaterial.color = color;
        mRenderer.material = mMaterial;
    }
#endif

    public override void OnObjectPlaced()
    {
        
    }
}
