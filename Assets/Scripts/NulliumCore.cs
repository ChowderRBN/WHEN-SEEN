using UnityEngine;

public class NulliumCore : MonoBehaviour
{
    [Header("Core ID")]
    public string coreID; // Unique ID for this core

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    private NulliumRadar radar;

    void Start()
    {
        // Generate ID if not set
        if (string.IsNullOrEmpty(coreID))
        {
            coreID = transform.position.ToString(); // Use position as unique ID
        }

        // Check if already collected
        if (GameManager.Instance != null && GameManager.Instance.IsCoreCollected(coreID))
        {
            Destroy(gameObject); // Already collected, don't spawn
            return;
        }

        // Register with radar
        radar = FindObjectOfType<NulliumRadar>();
        if (radar != null)
        {
            radar.RegisterNulliumCore(this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other.gameObject);
        }
    }

    void Pickup(GameObject player)
    {
        // Play pickup sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Add to inventory
        NulliumInventory inventory = player.GetComponent<NulliumInventory>();
        if (inventory != null)
        {
            inventory.AddNulliumCore(coreID);
        }

        // Mark as collected in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MarkCoreCollected(coreID);
        }

        // Unregister from radar
        if (radar != null)
        {
            radar.UnregisterNulliumCore(this);
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (radar != null)
        {
            radar.UnregisterNulliumCore(this);
        }
    }
}