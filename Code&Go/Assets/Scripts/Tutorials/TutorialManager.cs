using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private PopUpManager popUpManager;
    [SerializeField] private bool tutorialsON = true;
    private BinaryHeap<TutorialTrigger> priorTriggers = null;
    private List<TutorialTrigger> conditionTriggers = null;

    private HashSet<string> triggered = null;   // Stores the hash of all triggered tutorial
    private List<TutorialTrigger> savePending;  // Stores the pending triggers to be saved

    // Hash format
    private HashSet<string> saved = null; // Stores save data, loaded from file or modified on execution

    private bool needToBeDestroyed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            priorTriggers = new BinaryHeap<TutorialTrigger>();
            conditionTriggers = new List<TutorialTrigger>();

            triggered = new HashSet<string>();

            savePending = new List<TutorialTrigger>();
            saved = new HashSet<string>();

            return;
        }
        Instance.tutorialsON = tutorialsON;
        needToBeDestroyed = true;
    }

    private void Start()
    {
        if (needToBeDestroyed)
        {
            Instance.Start();
            Destroy(gameObject);
            return;
        }

        //TutorialTrigger[] aux = FindObjectsOfType<TutorialTrigger>();
        //for (int i = 0; i < aux.Length; i++)
        //{
        //    AddTutorialTrigger(aux[i], true);
        //}
        //conditionTriggers.Sort();

    }

    private void Update()
    {
        if (!tutorialsON) return;

        TutorialTrigger prior = TryPopPriorityTriggers();
        TutorialTrigger cond = TryPopConditionalTriggers();

        if (prior != null && cond != null)
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
        else if (prior != null)
        {
            ShowTutorialInfo(prior);
        }
        else if (cond != null)
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

        TutorialTrigger trigger = conditionTriggers[0];
        if (trigger.condition.Invoke()) return trigger;

        return null;
    }

    private void ShowTutorialInfo(TutorialTrigger t)
    {
        if (t == null) return;

        PopUpData info = t.info;
        if (t.highlightObject)
            popUpManager.Show(info, t.GetRect());
        else
            popUpManager.Show(info.title, info.content);
        if (TemaryManager.Instance != null)
            TemaryManager.Instance.AddTemary(t.info);

        if (t.OnShowed != null)
            t.OnShowed.Invoke();

        if (!triggered.Contains(t.GetHash()))
            triggered.Add(t.GetHash());

        if (!saved.Contains(t.GetHash()))
            savePending.Add(t);

        if (t.isSaveCheckpoint)
            SavePendingTriggers();

        if (t.destroyOnShowed)
            Destroy(t);

    }

    public void AddTutorialTrigger(TutorialTrigger t, bool checkTriggered = false)
    {
        if (checkTriggered && triggered.Contains(t.GetHash())) return;

        if (t.condition != null)
        {
            conditionTriggers.Add(t);
            conditionTriggers.Sort();
        }
        else
            priorTriggers.Add(t);
    }

    public HashSet<string> GetTriggeredTutorials()
    {
        return triggered;
    }

    public void Load(TutorialSaveData data)
    {
        for (int i = 0; i < data.tutorials.Length; i++)
        {
            saved.Add(data.tutorials[i]);
            triggered.Add(data.tutorials[i]);
        }
    }

    public TutorialSaveData Save()
    {
        string[] array = new string[saved.Count];
        saved.CopyTo(array);

        TutorialSaveData data = new TutorialSaveData();
        data.tutorials = array;
        return data;
    }

    private void SavePendingTriggers()
    {
        foreach (TutorialTrigger trigger in savePending)
        {
            string hash = trigger.GetHash();
            saved.Add(hash);
        }
        Save();

        savePending.Clear();
    }
}
