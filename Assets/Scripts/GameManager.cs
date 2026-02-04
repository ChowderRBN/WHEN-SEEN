using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Current Game Data")]
    public int currentSaveSlot = -1;
    public Vector3 playerPosition;
    public float playTime = 0f;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        // Track play time
        if (currentSaveSlot != -1)
        {
            playTime += Time.deltaTime;
        }
    }

    // Create a new game in a specific slot
    public void NewGame(int slot)
    {
        currentSaveSlot = slot;
        playTime = 0f;
        playerPosition = Vector3.zero;

        // Load Game1 scene (your cutscene scene)
        SceneManager.LoadScene("Game1");
    }

    // Save current game state
    public void SaveGame()
    {
        if (currentSaveSlot == -1)
        {
            Debug.LogWarning("No save slot selected!");
            return;
        }

        SaveData data = new SaveData();
        data.saveName = "Save " + currentSaveSlot;
        data.playerPosition = playerPosition;
        data.playTime = playTime;

        SaveSystem.SaveGame(currentSaveSlot, data);

        Debug.Log("Game saved to slot " + currentSaveSlot);
    }

    // Load game from a specific slot
    public void LoadGame(int slot)
    {
        SaveData data = SaveSystem.LoadGame(slot);

        if (data != null)
        {
            currentSaveSlot = slot;
            playerPosition = data.playerPosition;
            playTime = data.playTime;

            // Load Game1 scene
            SceneManager.LoadScene("Game1");

            Debug.Log("Game loaded from slot " + slot);
        }
        else
        {
            Debug.LogError("Failed to load save from slot " + slot);
        }
    }

    // Quick save (saves to current slot)
    public void QuickSave()
    {
        SaveGame();
    }
}