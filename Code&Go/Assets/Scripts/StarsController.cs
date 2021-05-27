using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StarsController : MonoBehaviour
{
    public Image minimumStepsStar;
    public Image firstRunStar;
    public Image noHintsStar;
    public Text stepsOverflowText;
    private Color deactivatedColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

    public uint GetStars()
    {
        uint nStars = 3;
        if (firstRunStar.color == deactivatedColor)
            nStars --;
        if (noHintsStar.color == deactivatedColor)
            nStars --;
        if (minimumStepsStar.color == deactivatedColor)
            nStars --;

        return nStars;
    }

    public void DeactivateFirstRunStar()
    {
        firstRunStar.color = deactivatedColor;
    }

    public void DeactivateNoHintsStar()
    {
        noHintsStar.color = deactivatedColor;
    }

    public void DeactivateMinimumStepsStar(int stepsOverflow)
    {
        minimumStepsStar.color = deactivatedColor;
        stepsOverflowText.text = stepsOverflow.ToString() + "+";
    }

    public bool IsFirstRunStarActive()
    {
        return firstRunStar.enabled;
    }

    public bool IsMinimumStepsStarActive()
    {
        return minimumStepsStar.enabled;
    }

    public bool IsNoHintsStarActive()
    {
        return noHintsStar.enabled;
    }
}
