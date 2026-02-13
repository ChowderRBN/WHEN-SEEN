using UnityEngine;

public class NulliumInventory : MonoBehaviour
{
    public int nulliumCores = 0;
    public int maxCores = 5;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip coreCollectedSound;

    public void AddNulliumCore()
    {
        if (nulliumCores < maxCores)
        {
            nulliumCores++;

            if (audioSource != null && coreCollectedSound != null)
            {
                audioSource.PlayOneShot(coreCollectedSound);
            }

            Debug.Log($"Nullium core collected! Total: {nulliumCores}/{maxCores}");

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