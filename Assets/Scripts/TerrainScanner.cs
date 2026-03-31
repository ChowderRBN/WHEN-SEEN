using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrainScanner : MonoBehaviour
{
    [Header("Echolocation Settings")]
    public GameObject particlePrefab;
    public float echoLifetime = 5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip echoSound;

    [Header("Noise")]
    public NoiseDetection noiseDetection;

    [Header("Input")]
    public InputActionAsset inputActionsAsset;

    private InputAction echoAction;

    private bool canEcho = true;

    void Start()
    {
        if (inputActionsAsset != null)
        {
            var playerMap = inputActionsAsset.FindActionMap("Player");
            echoAction = playerMap.FindAction("Echolocation");
        }
    }

    void OnEnable()
    {
        echoAction?.Enable();
    }

    void OnDisable()
    {
        echoAction?.Disable();
    }

    void Update()
    {
#if !UNITY_IOS && !UNITY_ANDROID
        if (echoAction != null && echoAction.triggered)
        {
            TryEcho();
        }
#endif
    }

    // MOBILE BUTTON SUPPORT
    public void SonarInput(bool pressed)
    {
        if (pressed)
        {
            TryEcho();
        }
    }

    
    public void SonarInput(Vector2 direction)
    {
        
        TryEcho();
    }

    private void TryEcho()
    {
        if (!canEcho) return;

        canEcho = false;
        SpawnTerrainScanner();

        // Small delay to prevent spam
        Invoke(nameof(ResetEcho), 0.3f);
    }

    private void ResetEcho()
    {
        canEcho = true;
    }

    public void SpawnTerrainScanner()
    {
        // Spawn visual
        if (particlePrefab != null)
        {
            GameObject echo = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(echo, echoLifetime);
        }

        // Play sound
        if (audioSource != null && echoSound != null)
        {
            audioSource.PlayOneShot(echoSound);
        }

        // Notify AI / noise system
        if (noiseDetection != null)
        {
            noiseDetection.NotifyEcholocationUsed();
        }
    }
}