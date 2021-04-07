using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TutorialSaveData
{
    public string[] tutorials;
    public string hash;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    private string filename = "tutorial_sd.json";
    private static string Filepath = "";

    private string dataPath = "";

    [SerializeField] private PopUpManager popUpManager;
    private BinaryHeap<TutorialTrigger> priorTriggers = null;
    private List<TutorialTrigger> conditionTriggers = null;

    private HashSet<string> triggered = null;   // Stores the hash of all triggered tutorial
    private List<TutorialTrigger> savePending;  // Stores the pending triggers to be saved

    // Hash format
    private HashSet<string> saved = null; // Stores save data, loaded from file or modified on execution

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dataPath =
#if UNITY_EDITOR
            Application.dataPath;
#else
            Application.persistentDataPath;
#endif
            Filepath = Path.Combine(dataPath, filename);

            priorTriggers = new BinaryHeap<TutorialTrigger>();
            conditionTriggers = new List<TutorialTrigger>();

            triggered = new HashSet<string>();

            savePending = new List<TutorialTrigger>();
            saved = new HashSet<string>();

            Load();

            return;
        }
        Destroy(gameObject);
    }

    private void Start()
    {
        TutorialTrigger[] aux = FindObjectsOfType<TutorialTrigger>();
        for(int i=0; i < aux.Length; i++)
        {
            AddTutorialTrigger(aux[i], true);
        }
        conditionTriggers.Sort();
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

        TutorialTrigger trigger = conditionTriggers[0];
        if (trigger.condition.Invoke()) return trigger;

        return null;
    }

    private void ShowTutorialInfo(TutorialTrigger t)
    {
        if (t == null) return;

        TutorialInfo info = t.info;
        if (t.highlightObject)
            popUpManager.Show(info.popUpData, t.GetRect());
        else
            popUpManager.Show(info.popUpData.title, info.popUpData.content);

        if (t.OnShowed != null)
            t.OnShowed.Invoke();

        if (!saved.Contains(t.GetHash())) { 
            savePending.Add(t);

        if(t.isSaveCheckpoint)
            SavePendingTriggers();}

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

   // TODO: modificar cuando se haga el sistema de guardado
    private void Load()
    {
        // Si no existe, se crea
        /*if (!File.Exists(Filepath))
        {
            FileStream file = new FileStream(Filepath, FileMode.Create);
            file.Close();
            Save();
            return;
        }

        StreamReader reader = new StreamReader(Filepath);
        string readerData = reader.ReadToEnd();
        reader.Close();

        // Leemos
        TutorialSaveData data = JsonUtility.FromJson<TutorialSaveData>(readerData);

        // Verificamos
        if (Hash.ToHash(data.tutorials.ToString(), "") == data.hash)
        {
            for (int i = 0; i < data.tutorials.Length; i++)
            {
                saved.Add(data.tutorials[i]);
                triggered.Add(data.tutorials[i]);
            }
            return;
        }
        // Se ha modificado el archivo, empiezas de 0
        Save();*/
    }

    // TODO: modificar cuando se haga el sistema de guardado
    private void Save()
    {/*
        string[] array = new string[saved.Count];
        saved.CopyTo(array);

        TutorialSaveData data = new TutorialSaveData();
        data.tutorials = array;
        data.hash = Hash.ToHash(array.ToString(), "");

        string finalJson = JsonUtility.ToJson(data);
        // Se crea de nuevo
        FileStream file = new FileStream(Filepath, FileMode.Create);
        file.Close();

        StreamWriter writer = new StreamWriter(Filepath);
        writer.Write(finalJson);
        writer.Close();*/
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
