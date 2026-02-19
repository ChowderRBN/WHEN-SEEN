using UnityEngine;
using TMPro;

public class MicrophoneDebug : MonoBehaviour
{
    public MicrophoneNoiseDetection micDetection;
    public TextMeshProUGUI debugText;

    void Update()
    {
        if (micDetection == null || debugText == null) return;

        debugText.text = $"Microphone: {(micDetection.useMicrophone ? "ON" : "OFF")}\n";
        debugText.text += $"Device: {micDetection.microphoneDevice}\n";
        debugText.text += $"Noise Level: {micDetection.currentNoiseLevel:F4}\n";
        debugText.text += $"Threshold: {micDetection.noiseThreshold:F4}\n";

        if (micDetection.currentNoiseLevel > micDetection.noiseThreshold)
        {
            debugText.text += "\n<color=red>MONSTERS ALERTED!</color>";
        }
    }
}