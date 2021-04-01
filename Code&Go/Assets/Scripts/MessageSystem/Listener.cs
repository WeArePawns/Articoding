using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Listener : MonoBehaviour
{
    public abstract void ReceiveMessage(string msg, MSG_TYPE type);

    private void Start()
    {
        MessageManager.Instance.SubscribeListener(this);
    }

    private void OnDestroy()
    {
        MessageManager.Instance.UnsubscribeListener(this);
    }
}
