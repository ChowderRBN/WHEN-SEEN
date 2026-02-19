using UnityEngine;

public class MicrophoneNoiseDetection : MonoBehaviour
{
    [Header("Microphone Settings")]
    public bool useMicrophone = true;
    public string microphoneDevice = null;
    public int sampleRate = 44100;
    public int sampleWindow = 128;

    [Header("Detection Settings")]
    public float noiseThreshold = 0.01f;
    public float maxNoiseLevel = 0.1f;
    public float noiseMultiplier = 10f;

    [Header("Cooldown")]
    public float detectionCooldown = 0.5f;
    public float lastDetectionTime = 0f;  

    [Header("UI Feedback")]
    public UnityEngine.UI.Image micIndicator;
    public Color quietColor = Color.green;
    public Color loudColor = Color.red;

    [Header("Audio Feedback")]
    public AudioSource feedbackAudio;
    public AudioClip detectionBeep;

    [Header("Noise Detection Reference")]
    public NoiseDetection noiseDetection;

    public AudioClip microphoneClip;  
    public bool isMicrophoneActive = false;  
    public float currentNoiseLevel = 0f;  

void Start()
    {
        if (useMicrophone)
        {
            InitializeMicrophone();
        }

        if (noiseDetection == null)
        {
            noiseDetection = GetComponent<NoiseDetection>();
        }
    }

    public void InitializeMicrophone() 
    {
        if (Microphone.devices.Length > 0)
        {
            if (string.IsNullOrEmpty(microphoneDevice))
            {
                microphoneDevice = Microphone.devices[0];
            }

            microphoneClip = Microphone.Start(microphoneDevice, true, 1, sampleRate);
            isMicrophoneActive = true;

            Debug.Log($"Microphone initialized: {microphoneDevice}");
        }
        else
        {
            Debug.LogWarning("No microphone detected!");
            useMicrophone = false;
        }
    }

    void Update()
    {
        if (!useMicrophone || !isMicrophoneActive) return;

        currentNoiseLevel = GetMicrophoneLoudness();

        UpdateMicIndicator();

        if (currentNoiseLevel > noiseThreshold && Time.time >= lastDetectionTime + detectionCooldown)
        {
            OnNoiseDetected(currentNoiseLevel);
            lastDetectionTime = Time.time;
        }
    }

    float GetMicrophoneLoudness()
    {
        if (microphoneClip == null) return 0f;

        int micPosition = Microphone.GetPosition(microphoneDevice) - (sampleWindow + 1);
        if (micPosition < 0) return 0f;

        float[] samples = new float[sampleWindow];
        microphoneClip.GetData(samples, micPosition);

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }

        return sum / sampleWindow;
    }

    void OnNoiseDetected(float noiseLevel)
    {
        float normalizedNoise = Mathf.Clamp(noiseLevel / maxNoiseLevel, 0f, 1f);
        float noiseAmount = normalizedNoise * noiseMultiplier;

        Debug.Log($"Microphone noise detected! Level: {noiseAmount:F2}/10");

        if (noiseDetection != null)
        {
            noiseDetection.AddNoise(noiseAmount);
        }

        if (feedbackAudio != null && detectionBeep != null)
        {
            feedbackAudio.PlayOneShot(detectionBeep, 0.3f);
        }
    }

    void UpdateMicIndicator()
    {
        if (micIndicator == null) return;

        float intensity = Mathf.Clamp01(currentNoiseLevel / noiseThreshold);
        micIndicator.color = Color.Lerp(quietColor, loudColor, intensity);

        if (currentNoiseLevel > noiseThreshold)
        {
            float pulse = Mathf.PingPong(Time.time * 5f, 1f);
            micIndicator.color = Color.Lerp(loudColor, Color.white, pulse * 0.3f);
        }
    }

    void OnDestroy()
    {
        if (isMicrophoneActive && !string.IsNullOrEmpty(microphoneDevice))
        {
            Microphone.End(microphoneDevice);
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause && isMicrophoneActive)
        {
            Microphone.End(microphoneDevice);
        }
        else if (!pause && useMicrophone)
        {
            InitializeMicrophone();
        }
    }
}