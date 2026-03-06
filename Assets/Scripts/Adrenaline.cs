using UnityEngine;
using System.Collections;

public class PlayerAdrenaline : MonoBehaviour
{
    [Header("References")]
    public FirstPersonController controller;
    public TerrainScanner terrainScanner;

    [Header("Proximity Trigger")]
    public float triggerDistance = 5f;

    [Header("Adrenaline Settings")]
    public float duration = 10f;
    public float speedMultiplier = 1.65f;
    public float cooldown = 5f; // Cooldown before can trigger again

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip adrenalineSound;

    private bool active = false;
    private float lastTriggerTime = 0f;

    void Start()
    {
        // Auto-find components
        if (controller == null)
        {
            controller = GetComponent<FirstPersonController>();
        }

        if (terrainScanner == null)
        {
            terrainScanner = GetComponent<TerrainScanner>();
        }
    }

    void Update()
    {
        // Don't check if already active or on cooldown
        if (active || Time.time < lastTriggerTime + cooldown) return;

        // Find all enemies with "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObj in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemyObj.transform.position);

            if (distance <= triggerDistance)
            {
                StartCoroutine(Activate());
                break;
            }
        }
    }

    IEnumerator Activate()
    {
        active = true;
        lastTriggerTime = Time.time;

        Debug.Log("ADRENALINE ACTIVATED!");

        // Visual feedback - auto echolocation
        if (terrainScanner != null)
        {
            terrainScanner.SpawnTerrainScanner();
        }

        // Speed boost
        if (controller != null)
        {
            controller.SetSpeedMultiplier(speedMultiplier);
        }

        // Play sound
        if (audioSource != null && adrenalineSound != null)
        {
            audioSource.PlayOneShot(adrenalineSound);
        }

        // Wait for duration
        yield return new WaitForSeconds(duration);

        // Reset speed
        if (controller != null)
        {
            controller.ResetSpeed();
        }

        Debug.Log("Adrenaline ended");
        active = false;
    }

    // Can be called manually (like from sonic scream)
    public void TriggerAdrenaline()
    {
        if (!active && Time.time >= lastTriggerTime + cooldown)
        {
            StartCoroutine(Activate());
        }
    }

    public bool IsActive()
    {
        return active;
    }
}