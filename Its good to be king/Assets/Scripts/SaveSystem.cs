using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string path => Application.persistentDataPath + "/save.json";

    public static void Save(PlayerHub playerHub)
    {
        SaveData data = new SaveData { playerHub = playerHub };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Saved: " + path);
    }
}
[System.Serializable]
public class SaveData
{
    public PlayerHub playerHub;
}
