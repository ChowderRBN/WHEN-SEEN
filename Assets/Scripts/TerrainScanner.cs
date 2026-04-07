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

    public void SonarInput(bool pressed)
    {
        if (pressed)
        {
            TryEcho();
        }
    }

    private void TryEcho()
    {
        if (!canEcho) return;

        canEcho = false;
        SpawnTerrainScanner();

        Invoke(nameof(ResetEcho), 0.3f);
    }

    private void ResetEcho()
    {
        canEcho = true;
    }

    public void SpawnTerrainScanner()
    {
        if (particlePrefab != null)
        {
            GameObject echo = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(echo, echoLifetime);
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