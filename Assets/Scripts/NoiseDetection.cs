using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoiseDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float currentNoise = 0f;
    public float maxNoise = 10f;
    public float noiseDecayRate = 1f;

    [Header("Movement Noise")]
    public float movementNoisePerUnit = 4f;
    public float crouchNoiseMultiplier = 0.3f;

    [Header("Ability Noise")]
    public float echolocationNoise = 8f;
    public float sonicScreamNoise = 10f;

    [Header("UI - Noise Meter")]
    public Image noiseMeterFill;
    public Image noiseMeterBackground;
    public Color lowNoiseColor = Color.green;
    public Color mediumNoiseColor = Color.yellow;
    public Color highNoiseColor = Color.red;
    public Text noiseText;

    [Header("Tutorial Warning (One-Time)")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;
    public string warningMessage = "CONTINUOUS NOISE WILL ATTRACT RESONATES TO YOUR LOCATION";
    public float warningDisplayTime = 3f;
    private bool hasShownWarningEver = false;

    [Header("Spawn Thresholds")]
    public float yellowThreshold = 6.6f;
    public float redThreshold = 8f;

    [Header("Monster Spawning")]
    public NoiseMonsterSpawner monsterSpawner;

    [Header("References")]
    public FirstPersonController playerController;
    public TerrainScanner terrainScanner;

    private Vector3 lastPosition;
    private bool isCrouching = false;
    private float lastSpawnTime = 0f;
    private float spawnCooldown = 5f;

    void Start()
    {
        lastPosition = transform.position;
        UpdateNoiseUI();

        if (warningPanel != null)
            warningPanel.SetActive(false);

        if (monsterSpawner == null)
            monsterSpawner = FindObjectOfType<NoiseMonsterSpawner>();

        hasShownWarningEver = PlayerPrefs.GetInt("NoiseWarningShown", 0) == 1;
    }

    void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > 0.01f)
        {
            float noiseGenerated = distanceMoved * movementNoisePerUnit;
            if (isCrouching) noiseGenerated *= crouchNoiseMultiplier;
            AddNoise(noiseGenerated);
        }

        if (isCrouching && distanceMoved < 0.01f)
            DecreaseNoise(noiseDecayRate * Time.deltaTime);

        UpdateNoiseUI();
        CheckNoiseThresholds();
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

    void CheckNoiseThresholds()
    {
        if (currentNoise >= yellowThreshold && !hasShownWarningEver)
        {
            ShowWarning();
            hasShownWarningEver = true;
            PlayerPrefs.SetInt("NoiseWarningShown", 1);
            PlayerPrefs.Save();
        }

        if (currentNoise >= redThreshold && Time.time >= lastSpawnTime + spawnCooldown)
        {
            SpawnMonsterFromNoise();
            lastSpawnTime = Time.time;
        }
    }

    void ShowWarning()
    {
        if (warningPanel != null && warningText != null)
        {
            warningPanel.SetActive(true);
            warningText.text = warningMessage;
            StartCoroutine(HideWarningAfterDelay());
        }

        Debug.Log("TUTORIAL: Noise attracting Resonates! (This message will only show once)");
    }

    System.Collections.IEnumerator HideWarningAfterDelay()
    {
        yield return new WaitForSeconds(warningDisplayTime);
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }

    void SpawnMonsterFromNoise()
    {
        if (monsterSpawner != null)
        {
            monsterSpawner.SpawnMonsterNearPlayer();
            Debug.Log("RED ALERT: Monster spawned due to noise!");
        }
    }

    void UpdateNoiseUI()
    {
        if (noiseMeterFill != null)
        {
            noiseMeterFill.fillAmount = currentNoise / maxNoise;

            if (currentNoise < yellowThreshold)
                noiseMeterFill.color = lowNoiseColor;
            else if (currentNoise < redThreshold)
                noiseMeterFill.color = mediumNoiseColor;
            else
                noiseMeterFill.color = highNoiseColor;
        }

        if (noiseText != null)
        {
            if (currentNoise >= redThreshold)
            {
                noiseText.text = "DANGER!";
                noiseText.color = highNoiseColor;
            }
            else if (currentNoise >= yellowThreshold)
            {
                noiseText.text = "CAUTION";
                noiseText.color = mediumNoiseColor;
            }
            else
            {
                noiseText.text = $"{Mathf.RoundToInt(currentNoise)}/{Mathf.RoundToInt(maxNoise)}";
                noiseText.color = Color.white;
            }
        }
    }

    void BroadcastNoise()
    {
        ResonateAI[] resonates = FindObjectsOfType<ResonateAI>();
        float detectionRadius = currentNoise * 5f;

        foreach (ResonateAI resonate in resonates)
        {
            float distance = Vector3.Distance(transform.position, resonate.transform.position);
            if (distance <= detectionRadius)
                resonate.HearNoise(transform.position, currentNoise);
        }
    }

    void BroadcastSonicScream()
    {
        ResonateAI[] resonates = FindObjectsOfType<ResonateAI>();
        foreach (ResonateAI resonate in resonates)
            resonate.Flee(transform.position, 50f);
    }

    public void NotifyEcholocationUsed() => OnEcholocationUsed();
    public void NotifySonicScreamUsed() => OnSonicScreamUsed();

    public void ResetTutorialWarning()
    {
        hasShownWarningEver = false;
        PlayerPrefs.SetInt("NoiseWarningShown", 0);
        PlayerPrefs.Save();
        Debug.Log("Tutorial warning reset - will show again");
    }
}