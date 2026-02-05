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

    void Start()
    {
        // Show main menu at start
        ShowMainMenu();

        // Load saved settings
        LoadSettings();
    }

    // ===== MAIN MENU BUTTONS =====

    public void NewGame()
    {
        PlayButtonClick();

        // Use GameManager to start new game (default slot 1, or modify as needed)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame(1);
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    public void LoadGame()
    {
        PlayButtonClick();
        ShowPanel(loadGamePanel);
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
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void ApplySettings()
    {
        PlayButtonClick();
        PlayerPrefs.Save();
        ShowMainMenu();
    }

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            float volume = PlayerPrefs.GetFloat("Volume");
            if (volumeSlider != null)
            {
                volumeSlider.value = volume;
            }
            AudioListener.volume = volume;
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            if (sensitivitySlider != null)
            {
                sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
            }
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = isFullscreen;
            }
            Screen.fullScreen = isFullscreen;
        }
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