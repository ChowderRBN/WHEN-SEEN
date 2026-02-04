using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuNavigator : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject loadGamePanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;

    [Header("Settings")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // Setup settings first (even if panel is disabled)
        SetupSettings();

        // Show main menu at start
        ShowMainMenu();

        // Load saved settings
        LoadSettings();
    }

    void SetupSettings()
    {
        // Temporarily enable settings panel to set it up
        bool wasActive = settingsPanel.activeSelf;
        if (!wasActive)
            settingsPanel.SetActive(true);

        // Setup resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Setup quality settings
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();

        // Restore original state
        if (!wasActive)
            settingsPanel.SetActive(false);
    }

    // ===== MAIN MENU BUTTONS =====

    public void NewGame()
    {
        PlayButtonClick();
        // Load your first game scene
        SceneManager.LoadScene("Game1"); // Change to your game scene name
    }

    public void LoadGame()
    {
        PlayButtonClick();
        ShowPanel(loadGamePanel);
        // You can implement save file loading here
    }

    public void ShowSettings()
    {
        PlayButtonClick();
        ShowPanel(settingsPanel);

        // Reload settings when opening settings panel
        LoadSettings();
    }

    public void ShowCredits()
    {
        PlayButtonClick();
        ShowPanel(creditsPanel);
    }

    public void QuitGame()
    {
        PlayButtonClick();
        Debug.Log("Quitting game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ===== NAVIGATION =====

    public void ShowMainMenu()
    {
        PlayButtonClick();
        ShowPanel(mainMenuPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        // Hide all panels
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (loadGamePanel != null) loadGamePanel.SetActive(false);

        // Show requested panel
        if (panelToShow != null) panelToShow.SetActive(true);
    }

    // ===== SETTINGS FUNCTIONS =====

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetSensitivity(float sensitivity)
    {
        // Save sensitivity for use in-game
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        Debug.Log("Sensitivity set to: " + sensitivity);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    public void ApplySettings()
    {
        PlayButtonClick();
        PlayerPrefs.Save();
        ShowMainMenu();
    }

    void LoadSettings()
    {
        // Temporarily disable slider events to prevent them from saving while loading
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveAllListeners();
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveAllListeners();

        // Load volume (default to 0.75 if not saved)
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

        // Load sensitivity (default to 2.0 if not saved)
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            float sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;
            Debug.Log("Loaded sensitivity: " + sensitivity);
        }
        else
        {
            if (sensitivitySlider != null) sensitivitySlider.value = 2.0f;
        }

        // Load fullscreen (default to true if not saved)
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }
        else
        {
            if (fullscreenToggle != null) fullscreenToggle.isOn = true;
            Screen.fullScreen = true;
        }

        // Load quality (default to highest if not saved)
        if (PlayerPrefs.HasKey("Quality"))
        {
            int quality = PlayerPrefs.GetInt("Quality");
            if (qualityDropdown != null) qualityDropdown.value = quality;
            QualitySettings.SetQualityLevel(quality);
        }
        else
        {
            int defaultQuality = QualitySettings.names.Length - 1;
            if (qualityDropdown != null) qualityDropdown.value = defaultQuality;
            QualitySettings.SetQualityLevel(defaultQuality);
        }

        // Load resolution
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionIndex");
            if (resolutionDropdown != null) resolutionDropdown.value = resIndex;
        }

        // Re-add the listeners after loading
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(SetVolume);
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);

        // Ensure sliders are interactable
        if (volumeSlider != null) volumeSlider.interactable = true;
        if (sensitivitySlider != null) sensitivitySlider.interactable = true;

        Debug.Log("Settings loaded. Sensitivity slider interactable: " + (sensitivitySlider != null ? sensitivitySlider.interactable.ToString() : "null"));
    }

    // ===== LOAD GAME FUNCTIONS =====

    public void LoadSaveSlot(int slotNumber)
    {
        PlayButtonClick();
        // Implement your save loading logic here
        Debug.Log("Loading save slot: " + slotNumber);
        // Example: SaveSystem.LoadGame(slotNumber);
        // SceneManager.LoadScene("GameScene");
    }

    public void DeleteSaveSlot(int slotNumber)
    {
        PlayButtonClick();
        // Implement your save deletion logic here
        Debug.Log("Deleting save slot: " + slotNumber);
        // Example: SaveSystem.DeleteSave(slotNumber);
    }

    // ===== AUDIO =====

    public void PlayButtonClick()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlayButtonHover()
    {
        if (audioSource != null && buttonHoverSound != null)
        {
            audioSource.PlayOneShot(buttonHoverSound);
        }
    }
}