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
        }
        // When game scene (Game2) loads from new game (after cutscene)
        else if (scene.name == gameScene && !isLoadingFromSave)
        {
            FindPlayerInScene();
            hasSeenIntroCutscene = true;
        }
        // When cutscene scene (Game1) loads, update the CutsceneManager's next scene
        else if (scene.name == cutsceneScene)
        {
            CutsceneManager cutsceneManager = FindObjectOfType<CutsceneManager>();
            if (cutsceneManager != null)
            {
                cutsceneManager.nextSceneName = gameScene;
            }
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
        // Track play time
        if (currentSaveSlot != -1)
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
        playTime = 0f;
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
        hasSeenIntroCutscene = false; // Reset this flag for new game
        isLoadingFromSave = false;

        // Check if this save slot already exists
        SaveData existingData = SaveSystem.LoadGame(slot);

        if (existingData != null && existingData.hasSeenIntroCutscene)
        {
            // If save exists and cutscene was seen, skip cutscene
            Debug.Log("Save exists with cutscene already seen, skipping to game");
            hasSeenIntroCutscene = true;
            SceneManager.LoadScene(gameScene);
        }
        else
        {
            // Brand new game, play cutscene
            Debug.Log("Brand new game, playing cutscene");
            hasSeenIntroCutscene = false;
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
        data.playTime = playTime;
        data.hasSeenIntroCutscene = true; // Mark cutscene as seen

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
            playerRotation = data.playerRotation;
            playTime = data.playTime;
            hasSeenIntroCutscene = data.hasSeenIntroCutscene;
            isLoadingFromSave = true;

            // Skip Game1 (cutscene) and load directly to Game2 (gameplay)
            Debug.Log("Loading save - skipping cutscene, going straight to Game2");
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