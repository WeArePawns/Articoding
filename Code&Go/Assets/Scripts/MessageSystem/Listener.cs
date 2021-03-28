using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Listener : MonoBehaviour
{
    public GameObject testPrefab;
    //Equivalente a uniones?
    public void ReceiveMessage(string msg, MSG_TYPE type)
    {
        switch (type)
        {
            case MSG_TYPE.INSTANTIATE:
                int nObjects = Int32.Parse(msg);
                for (int i = 0; i < nObjects; i++)
                    Instantiate(testPrefab);
                break;
            case MSG_TYPE.MOVE:
                string tag = msg.Split(' ')[0];
                int move = Int32.Parse(msg.Split(' ')[1]);
                GameObject go = GameObject.FindGameObjectWithTag(tag);
                if (go != null)
                    go.transform.position = go.transform.position + Vector3.right * move;
                break;
        }
    }

    private void Start()
    {
        MessageManager.Instance.SubscribeListener(this);
    }

    private void OnDestroy()
    {
        MessageManager.Instance.UnsubscribeListener(this);
    }
}
