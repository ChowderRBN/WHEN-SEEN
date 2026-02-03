using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public Dropdown qualityDropdown;
    public Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // Show main menu at start
        ShowMainMenu();

        // Setup settings
        SetupSettings();

        // Load saved settings
        LoadSettings();
    }

    void SetupSettings()
    {
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
    }

    // ===== MAIN MENU BUTTONS =====

    public void NewGame()
    {
        PlayButtonClick();
        // Load your first game scene
        SceneManager.LoadScene("GameScene"); // Change to your game scene name
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
        // Load volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            float volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = volume;
            AudioListener.volume = volume;
        }

        // Load sensitivity
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }

        // Load fullscreen
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        // Load quality
        if (PlayerPrefs.HasKey("Quality"))
        {
            int quality = PlayerPrefs.GetInt("Quality");
            qualityDropdown.value = quality;
            QualitySettings.SetQualityLevel(quality);
        }

        // Load resolution
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = resIndex;
        }
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