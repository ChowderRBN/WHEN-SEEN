using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Pause Menu Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Hide all panels at start
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Load settings
        LoadSettings();
    }

    void Update()
    {
        // Press ESC to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowPanel(pauseMenuPanel);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f; // Unfreeze game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HideAllPanels();
    }

    public void SaveGame()
    {
        PlayButtonClick();

        if (GameManager.Instance != null)
        {
            // Update player position before saving
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GameManager.Instance.playerPosition = player.transform.position;
            }

            GameManager.Instance.SaveGame();
            Debug.Log("Game Saved!");
        }
    }

    public void ShowSettings()
    {
        PlayButtonClick();
        ShowPanel(settingsPanel);
        LoadSettings();
    }

    public void BackToPauseMenu()
    {
        PlayButtonClick();
        ShowPanel(pauseMenuPanel);
    }

    public void QuitToMainMenu()
    {
        PlayButtonClick();
        Time.timeScale = 1f; // Restore time
        SceneManager.LoadScene("MainMenu"); // Your main menu scene name
    }

    void ShowPanel(GameObject panelToShow)
    {
        HideAllPanels();
        if (panelToShow != null) panelToShow.SetActive(true);
    }

    void HideAllPanels()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // ===== SETTINGS =====

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        Debug.Log("Sensitivity set to: " + sensitivity);

        // Update player sensitivity in real-time
        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.UpdateSensitivity();
        }
    }

    public void ApplySettings()
    {
        PlayButtonClick();
        PlayerPrefs.Save();
        BackToPauseMenu();
    }

    void LoadSettings()
    {
        // Disable listeners temporarily
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveAllListeners();
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveAllListeners();

        // Load volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            float volume = PlayerPrefs.GetFloat("Volume");
            if (volumeSlider != null) volumeSlider.value = volume;
            AudioListener.volume = volume;
        }
        else
        {
            if (volumeSlider != null) volumeSlider.value = 0.75f;
            AudioListener.volume = 0.75f;
        }

        // Load sensitivity
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            float sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;
        }
        else
        {
            if (sensitivitySlider != null) sensitivitySlider.value = 2.0f;
        }

        // Re-add listeners
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(SetVolume);
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    void PlayButtonClick()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}