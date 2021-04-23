using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StarsController : MonoBehaviour
{
    public Image minimoStar;
    public Image primeraEjecucionStar;
    public Image noPistasStar;
    private Color deactivatedColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

    public uint GetStars()
    {
        uint nStars = 3;
        if (primeraEjecucionStar.color == deactivatedColor)
            nStars --;
        if (noPistasStar.color == deactivatedColor)
            nStars --;
        if (minimoStar.color == deactivatedColor)
            nStars --;

        return nStars;
    }

    public void DeactivatePrimeraEjecucionStar()
    {
        primeraEjecucionStar.color = deactivatedColor;
    }

    public void DeactivateNoPistasStar()
    {
        noPistasStar.color = deactivatedColor;
    }

    public void DeactivateMinimoStar()
    {
        minimoStar.color = deactivatedColor;
    }
}
