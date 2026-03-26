using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Pause Menu Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;
    public GameObject settings;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    private bool isPaused = false;

    private Vector3 frozenPlayerPosition;
    private Quaternion frozenPlayerRotation;
    private GameObject cachedPlayer;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        LoadSettings();
    }

    void Update()
    {
        /*if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }*/
    }

    public void OnPause(InputValue input)
    {
        isPaused = !isPaused;
        if (isPaused == false)
            Resume();
        else
            Pause();
        Debug.Log("Pause button pressed. isPaused: " + isPaused);
    }


    public void Pause()
    {
        isPaused = true;

        // Find and freeze player in place
        cachedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (cachedPlayer != null)
        {
            frozenPlayerPosition = cachedPlayer.transform.position;
            frozenPlayerRotation = cachedPlayer.transform.rotation;

            // Zero out rigidbody physics if present
            Rigidbody rb = cachedPlayer.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // Disable all player scripts except PlayerInput and PauseManager
            MonoBehaviour[] scripts = cachedPlayer.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null
                    && !(script is PauseManager)
                    && !(script is PlayerInput))
                {
                    script.enabled = false;
                }
            }
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowPanel(pauseMenuPanel);
        EventSystem.current.SetSelectedGameObject(settings);
    }

    public void Resume()
    {
        isPaused = false;

        if (cachedPlayer != null)
        {
            // Snap player back to exact frozen position/rotation
            cachedPlayer.transform.position = frozenPlayerPosition;
            cachedPlayer.transform.rotation = frozenPlayerRotation;

            // Re-enable rigidbody
            Rigidbody rb = cachedPlayer.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;

            // Re-enable all player scripts
            MonoBehaviour[] scripts = cachedPlayer.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null)
                    script.enabled = true;
            }

            cachedPlayer = null;
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HideAllPanels();
    }

    public void SaveGame()
    {
        PlayButtonClick();

        if (GameManager.Instance != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                GameManager.Instance.playerPosition = player.transform.position;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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

        FirstPersonController player = FindObjectOfType<FirstPersonController>();
        if (player != null)
            player.UpdateSensitivity();
    }

    public void ApplySettings()
    {
        PlayButtonClick();
        PlayerPrefs.Save();
        BackToPauseMenu();
    }

    void LoadSettings()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveAllListeners();
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.RemoveAllListeners();

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

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            float sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;
        }
        else
        {
            if (sensitivitySlider != null) sensitivitySlider.value = 2.0f;
        }

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(SetVolume);
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    void PlayButtonClick()
    {
        if (audioSource != null && buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }
}