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

    [Header("Nullium Data")]
    public int nulliumCores = 0;
    public List<string> collectedCoreIDs = new List<string>();

    [Header("Player Info")]
    public string playerName = "Survivor";

    [Header("Scene Names")]
    public string cutsceneScene = "Game1";
    public string gameScene = "Game2";

    [Header("Player Reference")]
    public GameObject player;
    public FirstPersonController playerController;

    private bool isLoadingFromSave = false;
    private bool isPlayingGame = false;

    void Awake()
    {
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
        if (scene.name == gameScene && isLoadingFromSave)
        {
            StartCoroutine(LoadPlayerAfterDelay());
        }
        else if (scene.name == gameScene && !isLoadingFromSave)
        {
            FindPlayerInScene();
            hasSeenIntroCutscene = true;
            isPlayingGame = true;
        }
        else if (scene.name == cutsceneScene)
        {
            isPlayingGame = false;
            CutsceneManager cutsceneManager = FindObjectOfType<CutsceneManager>();
            if (cutsceneManager != null)
            {
                cutsceneManager.nextSceneName = gameScene;
            }
        }
        else
        {
            isPlayingGame = false;
        }
    }

    IEnumerator LoadPlayerAfterDelay()
    {
        yield return new WaitForEndOfFrame();

        FindPlayerInScene();
        yield return new WaitForEndOfFrame();

        LoadPlayerPosition();
        RestorePlayerState(); // updated version

        isLoadingFromSave = false;
        isPlayingGame = true;
    }

    void FindPlayerInScene()
    {
        player = null;
        playerController = null;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<FirstPersonController>();

            NulliumInventory inventory = player.GetComponent<NulliumInventory>();
            if (inventory != null)
            {
                inventory.nulliumCores = nulliumCores;
            }

            Debug.Log("Player found in scene!");
        }
        else
        {
            Debug.LogError("Could not find player in scene!");
        }
    }

    void LoadPlayerPosition()
    {
        if (player == null)
        {
            Debug.LogError("Cannot load position - player is null!");
            return;
        }

        Debug.Log($"=== LOADING PLAYER POSITION ===");
        Debug.Log($"Target position: {playerPosition}");
        Debug.Log($"Target rotation: {playerRotation.eulerAngles}");

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = playerPosition;
        player.transform.rotation = playerRotation;

        if (cc != null)
        {
            cc.enabled = true;
        }

        Debug.Log($"Player loaded at: {player.transform.position}");
        Debug.Log($"Player rotation: {player.transform.rotation.eulerAngles}");
    }

    // ===================== NEW RestorePlayerState =====================
    void RestorePlayerState()
    {
        if (player == null)
        {
            Debug.LogError("Cannot restore state - player is null!");
            return;
        }

        Debug.Log("=== RESTORING PLAYER STATE ===");

        // Reset noise detection
        NoiseDetection noiseDetection = player.GetComponent<NoiseDetection>();
        if (noiseDetection != null)
        {
            noiseDetection.currentNoise = 0f;
            noiseDetection.SetCrouching(false);
            Debug.Log("Noise reset to 0");
        }

        // Ensure player controller is enabled
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("Player controller enabled");
        }

        // Unpause game
        Time.timeScale = 1f;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Player state restored!");
    }
    // ================================================================

    void Update()
    {
        if (isPlayingGame && currentSaveSlot != -1)
        {
            playTime += Time.deltaTime;
        }

        if (player != null && isPlayingGame)
        {
            playerPosition = player.transform.position;
            playerRotation = player.transform.rotation;
        }

        if (Input.GetKeyDown(KeyCode.F5) && currentSaveSlot != -1 && isPlayingGame)
        {
            SaveGame();
        }
    }

    public void NewGame(int slot)
    {
        currentSaveSlot = slot;
        playTime = 0f;
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
        hasSeenIntroCutscene = false;
        isLoadingFromSave = false;
        nulliumCores = 0;
        collectedCoreIDs.Clear();

        SaveData existingData = SaveSystem.LoadGame(slot);

        if (existingData != null && existingData.hasSeenIntroCutscene)
        {
            Debug.Log("Save exists with cutscene already seen, skipping to game");
            hasSeenIntroCutscene = true;
            playTime = existingData.playTime;
            SceneManager.LoadScene(gameScene);
        }
        else
        {
            Debug.Log("Brand new game, playing cutscene");
            hasSeenIntroCutscene = false;
            playTime = 0f;
            SceneManager.LoadScene(cutsceneScene);
        }
    }

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
        data.hasSeenIntroCutscene = true;
        data.nulliumCores = nulliumCores;
        data.collectedCoreIDs = new List<string>(collectedCoreIDs);

        SaveSystem.SaveGame(currentSaveSlot, data);

        Debug.Log($"=== GAME SAVED ===");
        Debug.Log($"Position: {playerPosition}");
        Debug.Log($"Rotation: {playerRotation.eulerAngles}");
        Debug.Log($"Cores: {nulliumCores}");
        Debug.Log($"Collected Core IDs: {collectedCoreIDs.Count}");
    }

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
            nulliumCores = data.nulliumCores;

            if (data.collectedCoreIDs != null)
            {
                collectedCoreIDs = new List<string>(data.collectedCoreIDs);
            }
            else
            {
                collectedCoreIDs = new List<string>();
            }

            isLoadingFromSave = true;

            Debug.Log($"=== GAME LOADED ===");
            Debug.Log($"Position: {playerPosition}");
            Debug.Log($"Rotation: {playerRotation.eulerAngles}");
            Debug.Log($"Cores: {nulliumCores}");
            Debug.Log($"Collected Core IDs: {collectedCoreIDs.Count}");

            SceneManager.LoadScene(gameScene);
        }
        else
        {
            Debug.LogError("Failed to load save from slot " + slot);
        }
    }

    public void QuickSave()
    {
        SaveGame();
    }

    public void MarkCoreCollected(string coreID)
    {
        if (!collectedCoreIDs.Contains(coreID))
        {
            collectedCoreIDs.Add(coreID);
            Debug.Log($"Core marked as collected: {coreID}");
        }
    }

    public bool IsCoreCollected(string coreID)
    {
        return collectedCoreIDs.Contains(coreID);
    }
}