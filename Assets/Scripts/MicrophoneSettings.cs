using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MicrophoneSettings : MonoBehaviour
{
    [Header("References")]
    public MicrophoneNoiseDetection micDetection;

    [Header("UI")]
    public Toggle microphoneToggle;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;
    public TMP_Dropdown microphoneDropdown;

    void Start()
    {
        // Debug microphones
        Debug.Log($"Total microphones found: {Microphone.devices.Length}");
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log($"Microphone {i}: {Microphone.devices[i]}");
        }

        // Populate microphone list
        PopulateMicrophoneList();

        // Load saved settings
        LoadSettings();

        // Setup listeners
        if (microphoneToggle != null)
        {
            microphoneToggle.onValueChanged.AddListener(OnMicrophoneToggled);
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        if (microphoneDropdown != null)
        {
            microphoneDropdown.onValueChanged.AddListener(OnMicrophoneChanged);
        }
    }

    void PopulateMicrophoneList()
    {
        if (microphoneDropdown == null)
        {
            Debug.LogError("Microphone dropdown is not assigned!");
            return;
        }

        microphoneDropdown.ClearOptions();

        // Create simple string list
        List<string> deviceNames = new List<string>();

        if (Microphone.devices.Length > 0)
        {
            foreach (string device in Microphone.devices)
            {
                deviceNames.Add(device);
                Debug.Log($"Adding microphone option: {device}");
            }
        }
        else
        {
            deviceNames.Add("No Microphone Detected");
            Debug.LogWarning("No microphone devices detected!");
        }

        // Add strings directly
        microphoneDropdown.AddOptions(deviceNames);

        // Force update
        microphoneDropdown.RefreshShownValue();

        Debug.Log($"Added {deviceNames.Count} microphone options to dropdown");
    }

    void LoadSettings()
    {
        // Load microphone enabled
        bool micEnabled = PlayerPrefs.GetInt("MicrophoneEnabled", 1) == 1;
        if (microphoneToggle != null)
        {
            microphoneToggle.isOn = micEnabled;
        }

        // Load sensitivity
        float sensitivity = PlayerPrefs.GetFloat("MicrophoneSensitivity", 0.01f);
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = sensitivity * 100f;
        }

        UpdateSensitivityText(sensitivity * 100f);

        // Load selected microphone
        string savedMic = PlayerPrefs.GetString("SelectedMicrophone", "");
        if (!string.IsNullOrEmpty(savedMic) && microphoneDropdown != null)
        {
            for (int i = 0; i < Microphone.devices.Length; i++)
            {
                if (Microphone.devices[i] == savedMic)
                {
                    microphoneDropdown.value = i;
                    microphoneDropdown.RefreshShownValue();
                    break;
                }
            }
        }
    }

    void OnMicrophoneToggled(bool enabled)
    {
        Debug.Log($"Microphone toggled: {enabled}");

        if (micDetection != null)
        {
            micDetection.useMicrophone = enabled;

            if (enabled)
            {
                micDetection.InitializeMicrophone();
            }
            else
            {
                // Stop microphone
                if (!string.IsNullOrEmpty(micDetection.microphoneDevice))
                {
                    Microphone.End(micDetection.microphoneDevice);
                    micDetection.isMicrophoneActive = false;
                }
            }
        }

        PlayerPrefs.SetInt("MicrophoneEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnSensitivityChanged(float value)
    {
        float sensitivity = value / 100f;

        if (micDetection != null)
        {
            micDetection.noiseThreshold = sensitivity;
        }

        UpdateSensitivityText(value);

        PlayerPrefs.SetFloat("MicrophoneSensitivity", sensitivity);
        PlayerPrefs.Save();
    }

    void UpdateSensitivityText(float value)
    {
        if (sensitivityText != null)
        {
            sensitivityText.text = $"Sensitivity: {value:F0}%";
        }
    }

    void OnMicrophoneChanged(int index)
    {
        if (index >= 0 && index < Microphone.devices.Length)
        {
            string selectedMic = Microphone.devices[index];

            Debug.Log($"Microphone changed to: {selectedMic} (index {index})");

            if (micDetection != null)
            {
                // Stop current microphone
                if (micDetection.isMicrophoneActive && !string.IsNullOrEmpty(micDetection.microphoneDevice))
                {
                    Microphone.End(micDetection.microphoneDevice);
                }

                // Start new microphone
                micDetection.microphoneDevice = selectedMic;
                if (micDetection.useMicrophone)
                {
                    micDetection.InitializeMicrophone();
                }
            }

            PlayerPrefs.SetString("SelectedMicrophone", selectedMic);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning($"Invalid microphone index: {index}");
        }
    }
}