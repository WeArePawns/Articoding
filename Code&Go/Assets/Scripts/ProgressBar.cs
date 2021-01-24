using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    public float minimum;
    public float maximum;
    public float current;
    [SerializeField] private Color fillColor;
    [SerializeField] private Image mask;
    [SerializeField] private Image fill;

#if !UNITY_EDITOR
    private void Awake()
    {
        Configure();
    }
#else
    private void Update()
    {
        //Configure();
    }
#endif

    private void Configure()
    {
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillRatio = currentOffset / maximumOffset;
        mask.fillAmount = fillRatio;

        fill.color = fillColor;
    }

    public float GetMinimum()
    {
        return minimum;
    }

    public float GetMaximum()
    {
        return maximum;
    }

    public float GetCurrent()
    {
        return current;
    }
}
