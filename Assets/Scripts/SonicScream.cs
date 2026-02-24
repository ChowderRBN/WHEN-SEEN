using UnityEngine;
using System.Collections;

public class PlayerSonicScream : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;
    public AudioClip clip;

    [Header("Scream Settings")]
    public int maxScreams = 2;
    public float cooldown = 60f;
    public float fleeDistance = 50f; // How far enemies flee
    public float spawnBlockDuration = 5f; // Prevent spawns for 5 seconds

    [Header("Noise Detection")]
    public NoiseDetection noiseDetection;

    [Header("Spawner")]
    public NoiseMonsterSpawner spawner; // Reference to your spawn manager

    private int screams;

    void Start()
    {
        screams = maxScreams;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && screams > 0)
        {
            // Play scream sound
            if (source != null && clip != null)
            {
                source.PlayOneShot(clip);
            }

            // Make all enemies with the tag "Enemy" flee
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemyObj in enemies)
            {
                ResonateAI r = enemyObj.GetComponent<ResonateAI>();
                if (r != null)
                {
                    r.Flee(transform.position, fleeDistance);
                }
            }

            // Notify noise detection system
            if (noiseDetection != null)
            {
                noiseDetection.NotifySonicScreamUsed();
            }

            // Prevent new monsters from spawning for a few seconds
            if (spawner != null)
            {
                spawner.BlockSpawning(spawnBlockDuration);
            }

            screams--;
            StartCoroutine(Recharge());
        }
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(cooldown);
        screams = Mathf.Min(screams + 1, maxScreams);
    }
}