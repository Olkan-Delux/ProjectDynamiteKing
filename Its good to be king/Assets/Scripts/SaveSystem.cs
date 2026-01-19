using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    static string path => Application.persistentDataPath + "/save.json";

    public static void Save(PlayerHub playerHub, GameHub gameHub)
    {

        SaveData data = new SaveData { playerCharacter = playerHub.myCharacter, playerInteraction = playerHub.myInteraction, playerHeir = playerHub.myHeir, allCharacters = gameHub.allCharacters, nameRegion = gameHub.GetRegion() };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Saved: " + path);
    }

    public static SaveData Load()
    {

        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
[System.Serializable]
public class SaveData
{
    public Character playerCharacter;
    public Character playerInteraction;
    public Character playerHeir;
    public List<Character> allCharacters;
    public NameRegion nameRegion;
}
