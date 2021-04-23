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
        uint nStars = 0x7;
        if (primeraEjecucionStar.color == deactivatedColor)
            nStars &= 0x3;
        if (noPistasStar.color == deactivatedColor)
            nStars &= 0x5;
        if (minimoStar.color == deactivatedColor)
            nStars &= 0x6;

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
