using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : BoardObject, ILaserReceiver
{
    public void Receive()
    {
        throw new System.NotImplementedException();
    }

    public void OnLaserReceived()
    {
        throw new System.NotImplementedException();
    }

    // DO STUFF
    private void OnLaserGained()
    {

    }

    private void OnLaserLost()
    {

    }
}
