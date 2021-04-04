using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Listener : MonoBehaviour
{
    public abstract void ReceiveMessage(string msg, MSG_TYPE type);

    private void Start()
    {
        if (MessageManager.Instance != null)
            MessageManager.Instance.SubscribeListener(this);
    }

    private void OnDestroy()
    {
        if (MessageManager.Instance != null)
            MessageManager.Instance.UnsubscribeListener(this);
    }
}
