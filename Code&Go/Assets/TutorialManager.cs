using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager current;

    [SerializeField] PopUpManager popUpManager;
    private BinaryHeap<TutorialTrigger> priorTriggers;
    private List<TutorialTrigger> conditionTriggers;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        priorTriggers = new BinaryHeap<TutorialTrigger>();
        conditionTriggers = new List<TutorialTrigger>();

        TutorialTrigger[] aux = FindObjectsOfType<TutorialTrigger>();
        for(int i=0; i < aux.Length; i++)
        {
            AddTutorialTrigger(aux[i]);
        }
    }

    private void Update()
    {
        TutorialTrigger prior = TryPopPriorityTriggers();
        TutorialTrigger cond = TryPopConditionalTriggers();

        if(prior != null && cond != null)
        {
            if (prior.CompareTo(cond) <= 0)
            {
                ShowTutorialInfo(prior);
                AddTutorialTrigger(cond);
                return;
            }

            ShowTutorialInfo(cond);
            AddTutorialTrigger(prior);
        }
        else if(prior != null)
        {
            ShowTutorialInfo(prior);
        }
        else if(cond != null)
        {
            ShowTutorialInfo(cond);
        }
    }


    private TutorialTrigger TryPopPriorityTriggers()
    {
        if (priorTriggers.Count == 0) return null;
        if (popUpManager == null) return null;
        if (popUpManager.IsShowing()) return null;

        return priorTriggers.Remove();
    }

    private TutorialTrigger TryPopConditionalTriggers()
    {
        if (conditionTriggers.Count == 0) return null;
        if (popUpManager == null) return null;
        if (popUpManager.IsShowing()) return null;

        conditionTriggers.Sort();
        TutorialTrigger trigger = conditionTriggers[0];
        if (trigger.condition.Invoke()) return trigger;

        return null;
    }

    private void ShowTutorialInfo(TutorialTrigger t)
    {
        if (t == null) return;

        TutorialInfo info = t.info;
        popUpManager.Show(info.popUpData, t.GetRect());

        if(t.isSaveCheckpoint)
        {
            // Do stuff
        }

        if (t.destroyOnShowed)
            Destroy(t);

    }

    public void AddTutorialTrigger(TutorialTrigger t)
    {
        if (t.condition != null)
            conditionTriggers.Add(t);
        else
            priorTriggers.Add(t);
    }

}
