using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrainScanner : MonoBehaviour
{
    [Header("Echolocation Settings")]
    public GameObject particlePrefab;
    public float echoRadius = 50f;
    public float echoSpeed = 10f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip echoSound;

    [Header("Noise")]
    public NoiseDetection noiseDetection;

    [Header("Input")]
    public InputActionAsset inputActionsAsset;

    private InputAction echoAction;
    private bool canEcho = true;

    void OnEnable()
    {
        if (inputActionsAsset != null)
        {
            echoAction = inputActionsAsset.FindActionMap("Player").FindAction("Echolocation");
            if (echoAction != null)
            {
                echoAction.performed += HandleEcholocation;
                echoAction.canceled += HandleEchoReleased;
                echoAction.Enable();
            }
        }
    }

    void OnDisable()
    {
        if (echoAction != null)
        {
            echoAction.performed -= HandleEcholocation;
            echoAction.canceled -= HandleEchoReleased;
            echoAction.Disable();
        }
    }

    private void HandleEcholocation(InputAction.CallbackContext context)
    {
        if (!canEcho) return;
        canEcho = false;
        SpawnTerrainScanner();
    }

    private void HandleEchoReleased(InputAction.CallbackContext context)
    {
        canEcho = true;
    }

    public void SonarInput(bool sonarState)
    {
               if (sonarState && canEcho)
        {
            canEcho = false;
            SpawnTerrainScanner();
        }
        else
        {
            canEcho = true;
        }
    }
    public void SpawnTerrainScanner()
    {
        if (particlePrefab != null)
        {
            GameObject echo = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(echo, 5f);
        }

        if (audioSource != null && echoSound != null)
        {
            audioSource.PlayOneShot(echoSound);
        }

        if (noiseDetection != null)
        {
            noiseDetection.NotifyEcholocationUsed();
        }
    }

    internal void SonarInput(Vector2 virtualSonarDirection)
    {
        throw new NotImplementedException();
    }
}