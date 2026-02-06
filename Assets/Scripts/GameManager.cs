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
    public Quaternion playerRotation;
    public float playTime = 0f;
    public bool hasSeenIntroCutscene = false;

    [Header("Scene Names")]
    public string cutsceneScene = "Game1";
    public string gameScene = "Game2";

    [Header("Player Reference")]
    public GameObject player;
    public FirstPersonController playerController;

    private bool isLoadingFromSave = false;
    private bool isPlayingGame = false; // NEW: Track if we're actually in gameplay

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When the game scene (Game2) loads after loading a save
        if (scene.name == gameScene && isLoadingFromSave)
        {
            FindPlayerInScene();
            LoadPlayerPosition();
            isLoadingFromSave = false;
            isPlayingGame = true; // Start tracking time
        }
        // When game scene (Game2) loads from new game (after cutscene)
        else if (scene.name == gameScene && !isLoadingFromSave)
        {
            FindPlayerInScene();
            hasSeenIntroCutscene = true;
            isPlayingGame = true; // Start tracking time
        }
        // When cutscene scene (Game1) loads, update the CutsceneManager's next scene
        else if (scene.name == cutsceneScene)
        {
            isPlayingGame = false; // Don't track time during cutscene
            CutsceneManager cutsceneManager = FindObjectOfType<CutsceneManager>();
            if (cutsceneManager != null)
            {
                cutsceneManager.nextSceneName = gameScene;
            }
        }
        else
        {
            // In menu or other scenes, don't track time
            isPlayingGame = false;
        }
    }

    void FindPlayerInScene()
    {
        // Find player in scene if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<FirstPersonController>();
            }
        }
    }

    void LoadPlayerPosition()
    {
        if (player != null)
        {
            Debug.Log($"Loading player to position: {playerPosition}");
            player.transform.position = playerPosition;
            player.transform.rotation = playerRotation;
        }
    }

    void Update()
    {
        // Track play time only when in gameplay scene AND have an active save slot
        if (isPlayingGame && currentSaveSlot != -1)
        {
            playTime += Time.deltaTime;
        }

        // Update player position for saving
        if (player != null)
        {
            playerPosition = player.transform.position;
            playerRotation = player.transform.rotation;
        }
    }

    // Create a new game in a specific slot
    public void NewGame(int slot)
    {
        currentSaveSlot = slot;
        playTime = 0f; // Reset play time for new game
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
        hasSeenIntroCutscene = false;
        isLoadingFromSave = false;

        // Check if this save slot already exists
        SaveData existingData = SaveSystem.LoadGame(slot);

        if (existingData != null && existingData.hasSeenIntroCutscene)
        {
            // If save exists and cutscene was seen, skip cutscene
            Debug.Log("Save exists with cutscene already seen, skipping to game");
            hasSeenIntroCutscene = true;
            playTime = existingData.playTime; // Continue from saved time
            SceneManager.LoadScene(gameScene);
        }
        else
        {
            // Brand new game, play cutscene
            Debug.Log("Brand new game, playing cutscene");
            hasSeenIntroCutscene = false;
            playTime = 0f; // Fresh start
            SceneManager.LoadScene(cutsceneScene);
        }
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
        data.playerRotation = playerRotation;
        data.playTime = playTime; // Save current play time
        data.hasSeenIntroCutscene = true;

        SaveSystem.SaveGame(currentSaveSlot, data);
        Debug.Log($"Game saved to slot {currentSaveSlot} with play time: {playTime}");
    }

    // Load game from a specific slot
    public void LoadGame(int slot)
    {
        SaveData data = SaveSystem.LoadGame(slot);

        if (data != null)
        {
            currentSaveSlot = slot;
            playerPosition = data.playerPosition;
            playerRotation = data.playerRotation;
            playTime = data.playTime; // Load the saved play time
            hasSeenIntroCutscene = data.hasSeenIntroCutscene;
            isLoadingFromSave = true;

            Debug.Log($"Loading save from slot {slot} with play time: {playTime}");
            SceneManager.LoadScene(gameScene);
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