using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
    [SerializeField] PopUpManager popUpManager;
   // public static TutorialManager Instance;
    private BinaryHeap<TutotialInfoHolder> holders;

    private void Awake()
    {
        holders = new BinaryHeap<TutotialInfoHolder>();

        TutotialInfoHolder[] aux = FindObjectsOfType<TutotialInfoHolder>();
        for(int i=0; i < aux.Length; i++)
        {
            holders.Add(aux[i]);
        }
    }

    private void Update()
    {
        TryPopInitialHolders();
    }


    private void TryPopInitialHolders()
    {
        if (holders.Count == 0) return;
        if (popUpManager == null) return;
        if (popUpManager.IsShowing()) return;

        TutotialInfoHolder infoHolder = holders.Remove();
        TutorialInfo info = infoHolder.info;

        popUpManager.Show(info.popUpData, infoHolder.GetRect());
    }

}
