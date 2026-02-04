using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class SaveData
{
    public string saveName;
    public string saveDate;
    public int saveSlot;

    // Game progress data
    public Vector3 playerPosition;
    public float playTime;
}

public class SaveSystem : MonoBehaviour
{
    private static string GetSavePath(int slot)
    {
        return Application.persistentDataPath + "/save_slot_" + slot + ".json";
    }

    // Save game to a specific slot
    public static void SaveGame(int slot, SaveData data)
    {
        data.saveSlot = slot;
        data.saveDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);

        Debug.Log("Game saved to slot " + slot);
    }

    // Load game from a specific slot
    public static SaveData LoadGame(int slot)
    {
        string path = GetSavePath(slot);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded from slot " + slot);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in slot " + slot);
            return null;
        }
    }

    // Check if a save exists in a slot
    public static bool SaveExists(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    // Delete a save from a specific slot
    public static void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save deleted from slot " + slot);
        }
    }

    // Get all available saves
    public static List<SaveData> GetAllSaves()
    {
        List<SaveData> saves = new List<SaveData>();

        for (int i = 1; i <= 3; i++) // Check slots 1-3
        {
            if (SaveExists(i))
            {
                saves.Add(LoadGame(i));
            }
        }

        return saves;
    }
}