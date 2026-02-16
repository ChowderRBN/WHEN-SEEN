using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class NulliumRadar : MonoBehaviour
{
    [Header("Radar UI")]
    public RectTransform radarPanel; // The circular radar background
    public GameObject radarBlipPrefab; // Dot that represents Nullium
    public Image radarBackground;
    public TMP_Text coreCountText; // "Cores: 3/5"

    [Header("Radar Settings")]
    public float radarRadius = 100f; // Visual radius of radar in pixels
    public float maxDistance = 200f; // Maximum detection distance
    public float minDistance = 5f; // When core is this close, blip is at center
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

            // Get direction to core (flatten to 2D)
            Vector3 toCore = offset.normalized;

            // Calculate angle relative to player's forward direction
            // When facing the target, angle should be 0 (pointing forward/up on radar)
            float angle = Mathf.Atan2(toCore.x, toCore.z) * Mathf.Rad2Deg; // Target angle in world
            float playerAngle = Mathf.Atan2(player.forward.x, player.forward.z) * Mathf.Rad2Deg; // Player facing angle

            // Relative angle (0 = straight ahead, 90 = right, -90 = left, 180 = behind)
            float relativeAngle = angle - playerAngle;

            // Convert to radians for positioning
            float radians = relativeAngle * Mathf.Deg2Rad;

            // Calculate distance from center based on actual distance
            // Far away = edge of radar (radarRadius)
            // Very close = center (0)
            float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);
            float radarDistance = normalizedDistance * radarRadius;

            // Calculate radar position
            // X = left/right (sin gives horizontal position)
            // Y = up/down (cos gives vertical position - forward is UP)
            float x = Mathf.Sin(radians) * radarDistance;
            float y = Mathf.Cos(radians) * radarDistance; // Cos for Y makes forward = top

            Vector2 radarPosition = new Vector2(x, y);

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

            // Pulse effect (faster when closer)
            float pulseSpeed = Mathf.Lerp(blipPulseSpeed * 2f, blipPulseSpeed, normalizedDistance);
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float scale = Mathf.Lerp(0.8f, 1.2f, pulse);
            blipRect.localScale = Vector3.one * scale;

            radarBlips.Add(blip);
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