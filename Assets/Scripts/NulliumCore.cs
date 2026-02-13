using UnityEngine;

public class NulliumCore : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;

    private NulliumRadar radar;

    void Start()
    {
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
            inventory.AddNulliumCore();
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