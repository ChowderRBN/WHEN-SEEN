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

    private InputActionAsset inputActions;

    void Awake()
    {
        inputActions = new InputActionAsset();
    }

    void OnEnable()
    {
        //inputActions.Enable();
        //inputActions.Echolocation.performed += OnEcholocation;
    }

    void OnDisable()
    {
        //inputActions.Disable();
        //inputActions.Player.Echolocation.performed -= OnEcholocation;
    }

    void OnEcholocation(InputValue context)
    {
        SpawnTerrainScanner();
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
}