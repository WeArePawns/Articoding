using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColoredCell : BoardCell
{
    [SerializeField] protected Color color;
    [SerializeField] protected Material cellMaterial;

    [SerializeField] protected Renderer mRenderer;
    protected Material mMaterial;


    private void Awake()
    {
        if (mMaterial == null)
            mMaterial = new Material(cellMaterial);
        mMaterial.color = color;
        mRenderer.material = mMaterial;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (mMaterial == null)
            mMaterial = new Material(cellMaterial);
        mMaterial.color = color;
        mRenderer.material = mMaterial;
    }
#endif


    public override string[] GetArgs()
    {
        return new string[0];
    }

    public override void OnObjectPlaced() { }
}
