using UnityEngine;

public class CaveExitLightFlicker : MonoBehaviour
{
    public Light exitLight;
    public float baseIntensity = 0.5f;
    public float flickerAmount = 0.05f; // Very subtle
    public float flickerSpeed = 1.5f;

    void Start()
    {
        if (exitLight == null)
            exitLight = GetComponent<Light>();
    }

    void Update()
    {
        // Gentle flicker to simulate natural mineral glow
        float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerAmount;
        exitLight.intensity = baseIntensity + flicker;
    }
}