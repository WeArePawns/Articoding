using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public float minimum;
    public float maximum;
    public float current;
    public Color fillColor;
    public Image mask;
    public Image fill;

    private void Update()
    {
        GetCurrentFill();
    }

    private void GetCurrentFill()
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
