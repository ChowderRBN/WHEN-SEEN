using UnityEngine;

public class NulliumInventory : MonoBehaviour
{
    public int nulliumCores = 0;
    public int maxCores = 5;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coreCollectedSound;

    void Start()
    {
        // Load from GameManager if available
        if (GameManager.Instance != null)
        {
            nulliumCores = GameManager.Instance.nulliumCores;
        }
    }

    public void AddNulliumCore(string coreID)
    {
        if (nulliumCores < maxCores)
        {
            nulliumCores++;

            if (audioSource != null && coreCollectedSound != null)
            {
                audioSource.PlayOneShot(coreCollectedSound);
            }

            Debug.Log($"Nullium core collected! Total: {nulliumCores}/{maxCores}");

            // Update GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.nulliumCores = nulliumCores;
            }

            // Update radar display
            NulliumRadar radar = FindObjectOfType<NulliumRadar>();
            if (radar != null)
            {
                radar.UpdateCoreCount();
            }
        }
    }

    public int GetCoreCount()
    {
        return nulliumCores;
    }

    public bool HasAllCores()
    {
        return nulliumCores >= maxCores;
    }
}