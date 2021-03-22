using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MSG_TYPE { INSTANTIATE, MOVE};

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance = null;

    private List<Listener> listeners;

    private void Awake()
    {
        if (instance != null) return;

        instance = this;
        DontDestroyOnLoad(gameObject);
        listeners = new List<Listener>();
    }

    public void subscribeListener(Listener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void sendMessage(string msg, MSG_TYPE type)
    {
        foreach (Listener l in listeners)
            l.receiveMessage(msg, type);
    }
}
