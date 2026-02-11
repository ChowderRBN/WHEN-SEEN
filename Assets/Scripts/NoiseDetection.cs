using UnityEngine;
using UnityEngine.UI;

public class NoiseDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float currentNoise = 0f;
    public float maxNoise = 10f;
    public float noiseDecayRate = 1f; // Noise decreases per second when crouching

    [Header("Movement Noise")]
    public float movementNoisePerUnit = 4f; // 1 noise per 0.25 units = 4 per unit
    public float crouchNoiseMultiplier = 0.3f; // Reduce noise when crouching

    [Header("Ability Noise")]
    public float echolocationNoise = 8f;
    public float sonicScreamNoise = 10f; // Instant max detection

    [Header("UI")]
    public Slider noiseSlider;

    [Header("References")]
    public FirstPersonController playerController;
    public TerrainScanner terrainScanner;

    private Vector3 lastPosition;
    private bool isCrouching = false;

    void Start()
    {
        lastPosition = transform.position;

        if (noiseSlider != null)
        {
            noiseSlider.maxValue = maxNoise;
            noiseSlider.value = 0;
        }
    }

    void Update()
    {
        // Calculate movement noise
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > 0.01f)
        {
            float noiseGenerated = distanceMoved * movementNoisePerUnit;

            if (isCrouching)
            {
                noiseGenerated *= crouchNoiseMultiplier;
            }

            AddNoise(noiseGenerated);
        }

        // Decay noise when crouching and still
        if (isCrouching && distanceMoved < 0.01f)
        {
            DecreaseNoise(noiseDecayRate * Time.deltaTime);
        }

        // Update UI
        UpdateNoiseUI();

        // Broadcast noise to nearby enemies
        BroadcastNoise();

        lastPosition = transform.position;
    }

    public void AddNoise(float amount)
    {
        currentNoise = Mathf.Clamp(currentNoise + amount, 0, maxNoise);
    }

    public void DecreaseNoise(float amount)
    {
        currentNoise = Mathf.Clamp(currentNoise - amount, 0, maxNoise);
    }

    public void OnEcholocationUsed()
    {
        AddNoise(echolocationNoise);
        BroadcastNoise();
    }

    public void OnSonicScreamUsed()
    {
        AddNoise(sonicScreamNoise);
        BroadcastSonicScream();
    }

    public void SetCrouching(bool crouching)
    {
        isCrouching = crouching;
    }

    void UpdateNoiseUI()
    {
        if (noiseSlider != null)
        {
            noiseSlider.value = currentNoise;
        }
    }

    void BroadcastNoise()
    {
        // Find all Resonates in range and alert them based on noise level
        ResonateAI[] resonates = FindObjectsOfType<ResonateAI>();

        float detectionRadius = currentNoise * 5f; // Scale detection radius with noise

        foreach (ResonateAI resonate in resonates)
        {
            float distance = Vector3.Distance(transform.position, resonate.transform.position);

            if (distance <= detectionRadius)
            {
                resonate.HearNoise(transform.position, currentNoise);
            }
        }
    }

    void BroadcastSonicScream()
    {
        ResonateAI[] resonates = FindObjectsOfType<ResonateAI>();

        foreach (ResonateAI resonate in resonates)
        {
            resonate.Flee(transform.position, 50f);
        }
    }

    // Call this from your terrain scanner
    public void NotifyEcholocationUsed()
    {
        OnEcholocationUsed();
    }

    // Call this from your sonic scream ability
    public void NotifySonicScreamUsed()
    {
        OnSonicScreamUsed();
    }
}