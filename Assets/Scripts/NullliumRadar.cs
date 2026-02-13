using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NulliumRadar : MonoBehaviour
{
    [Header("Radar UI")]
    public RectTransform radarPanel; // The circular radar background
    public GameObject radarBlipPrefab; // Dot that represents Nullium
    public Image radarBackground;
    public Text coreCountText; // "Cores: 3/5"
    public Text distanceText; // Distance to nearest core

    [Header("Radar Settings")]
    public float radarRadius = 100f; // Visual radius of radar in pixels
    public float detectionRange = 200f; // How far radar can detect (in meters)
    public Color blipColor = new Color(0.3f, 1f, 0.3f); // Green blip
    public float blipSize = 10f; // Size of blip dots
    public float blipPulseSpeed = 2f; // How fast blips pulse

    [Header("Player")]
    public Transform player;
    public Image playerDot; // Center dot representing player

    [Header("Audio")]
    public AudioSource radarAudio;
    public AudioClip radarPing;
    public float pingInterval = 1f;

    private List<NulliumCore> activeCores = new List<NulliumCore>();
    private List<GameObject> radarBlips = new List<GameObject>();
    private float nextPingTime = 0f;

    void Start()
    {
        // Auto-find player
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Setup player dot in center
        if (playerDot != null)
        {
            playerDot.color = Color.white;
        }

        UpdateCoreCount();
    }

    void Update()
    {
        UpdateRadarBlips();

        // Ping sound
        if (Time.time >= nextPingTime && activeCores.Count > 0)
        {
            if (radarAudio != null && radarPing != null)
            {
                radarAudio.PlayOneShot(radarPing);
            }
            nextPingTime = Time.time + pingInterval;
        }

        // Update distance to closest core
        UpdateDistanceText();
    }

    void UpdateRadarBlips()
    {
        // Clear old blips
        foreach (GameObject blip in radarBlips)
        {
            Destroy(blip);
        }
        radarBlips.Clear();

        // Create new blips for each active core
        foreach (NulliumCore core in activeCores)
        {
            if (core == null) continue;

            // Calculate position relative to player
            Vector3 offset = core.transform.position - player.position;
            float distance = offset.magnitude;

            // Only show if within detection range
            if (distance > detectionRange) continue;

            // Flatten to 2D (top-down view)
            Vector2 radarPosition = new Vector2(offset.x, offset.z);

            // Rotate based on player's forward direction
            float playerAngle = Mathf.Atan2(player.forward.x, player.forward.z) * Mathf.Rad2Deg;
            radarPosition = RotateVector2(radarPosition, -playerAngle);

            // Scale to radar size
            radarPosition = radarPosition / detectionRange * radarRadius;

            // Clamp to radar bounds (circular)
            if (radarPosition.magnitude > radarRadius)
            {
                radarPosition = radarPosition.normalized * radarRadius;
            }

            // Create blip
            GameObject blip = Instantiate(radarBlipPrefab, radarPanel);
            RectTransform blipRect = blip.GetComponent<RectTransform>();
            blipRect.anchoredPosition = radarPosition;
            blipRect.sizeDelta = new Vector2(blipSize, blipSize);

            // Set color
            Image blipImage = blip.GetComponent<Image>();
            if (blipImage != null)
            {
                blipImage.color = blipColor;
            }

            // Pulse effect
            float pulse = (Mathf.Sin(Time.time * blipPulseSpeed) + 1f) * 0.5f;
            float scale = Mathf.Lerp(0.8f, 1.2f, pulse);
            blipRect.localScale = Vector3.one * scale;

            radarBlips.Add(blip);
        }
    }

    void UpdateDistanceText()
    {
        if (distanceText == null) return;

        // Find closest core
        float closestDistance = Mathf.Infinity;

        foreach (NulliumCore core in activeCores)
        {
            if (core == null) continue;

            float distance = Vector3.Distance(player.position, core.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        if (closestDistance < Mathf.Infinity)
        {
            distanceText.text = $"{Mathf.RoundToInt(closestDistance)}m";
        }
        else
        {
            distanceText.text = "---";
        }
    }

    public void UpdateCoreCount()
    {
        if (coreCountText != null)
        {
            NulliumInventory inventory = FindObjectOfType<NulliumInventory>();
            if (inventory != null)
            {
                coreCountText.text = $"Cores: {inventory.GetCoreCount()}/{inventory.maxCores}";
            }
        }
    }

    Vector2 RotateVector2(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public void RegisterNulliumCore(NulliumCore core)
    {
        if (!activeCores.Contains(core))
        {
            activeCores.Add(core);
            UpdateCoreCount();
        }
    }

    public void UnregisterNulliumCore(NulliumCore core)
    {
        activeCores.Remove(core);
        UpdateCoreCount();
    }
}