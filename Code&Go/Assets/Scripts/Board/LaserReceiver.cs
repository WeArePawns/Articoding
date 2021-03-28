using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : BoardObject, ILaserReceiver
{
    [SerializeField] private Renderer receiverRenderer;

    public void OnLaserReceived()
    {
        receiverRenderer.material.color = Color.red;
    }

    public void OnLaserReceiving()
    {
        // DO STUFF
    }

    public void OnLaserLost()
    {
        receiverRenderer.material.color = Color.yellow;
    }
}
