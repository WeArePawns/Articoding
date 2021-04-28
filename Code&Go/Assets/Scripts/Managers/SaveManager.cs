using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private static string filename = "gameSave.save";
    private static string Filepath = "";

    public static void Init()
    {
        string dataPath =
#if UNITY_EDITOR
        Application.dataPath;
#else
            Application.persistentDataPath;
#endif
        Filepath = Path.Combine(dataPath, filename);
    }

    public static void Load()
    {
        // Si no existe, se crea
        if (!File.Exists(Filepath))
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
        SaveData data = JsonUtility.FromJson<SaveData>(readerData);

        // Verificamos
        if (Hash.ToHash(data.gameData.ToString(), "") == data.hash)
        {
            TutorialManager.Instance.Load(data.gameData.tutorialInfo);
            ProgressManager.Instance.Load(data.gameData.progressData);
        }

        // Se ha modificado el archivo, empiezas de 0
        Save();
    }

    public static void Save()
    {
        SaveData data= new SaveData();
        GameSaveData gameData = new GameSaveData();
        gameData.tutorialInfo = TutorialManager.Instance.Save();
        gameData.progressData = ProgressManager.Instance.Save();
        data.gameData = gameData;

        data.hash = Hash.ToHash(data.gameData.ToString(), "");

        string finalJson = JsonUtility.ToJson(data);
        // Se crea de nuevo
        FileStream file = new FileStream(Filepath, FileMode.Create);
        file.Close();

        StreamWriter writer = new StreamWriter(Filepath);
        writer.Write(finalJson);
        writer.Close();
    }
}
