using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StarsController : MonoBehaviour
{
    public Image minimoStar;
    public Image primeraEjecucionStar;
    public Image noPistasStar;

    public GameObject continueButton;

    public int getStars()
    {
        int nStars = 3;
        if (!primeraEjecucionStar.gameObject.activeSelf)
            nStars--;
        if (!noPistasStar.gameObject.activeSelf)
            nStars--;
        if (!minimoStar.gameObject.activeSelf)
            nStars--;

        return nStars;
    }

    public void deactivatePrimeraEjecucionStar()
    {
        primeraEjecucionStar.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void deactivateNoPistasStar()
    {
        noPistasStar.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    public void deactivateMinimoStar()
    {
        minimoStar.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }
}
