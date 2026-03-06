using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NoiseMonsterSpawner : MonoBehaviour
{
    [Header("Monster Settings")]
    public GameObject monsterPrefab;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 20f;
    public float maxSpawnDistance = 30f;
    public int monstersPerSpawn = 1;

    [Header("Despawn Settings")]
    public float despawnDistance = 100f;
    public float despawnCheckInterval = 2f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    [Header("Player")]
    public Transform player;

    [Header("Spawn Blocking")]
    private bool spawnBlocked = false;
    private float spawnBlockEndTime = 0f;

    private List<GameObject> activeMonsters = new List<GameObject>();
    private float nextDespawnCheck = 0f;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // Clean up destroyed monsters
        activeMonsters.RemoveAll(monster => monster == null);

        // Check if spawn blocking is over
        if (spawnBlocked && Time.time >= spawnBlockEndTime)
        {
            spawnBlocked = false;
            Debug.Log("Spawn blocking ended");
        }

        // Periodically check for monsters to despawn
        if (Time.time >= nextDespawnCheck)
        {
            CheckForDespawn();
            nextDespawnCheck = Time.time + despawnCheckInterval;
        }
    }

    public void SpawnMonsterNearPlayer()
    {
        // Don't spawn if blocked
        if (spawnBlocked)
        {
            Debug.Log("Spawning blocked - skipping spawn");
            return;
        }

        for (int i = 0; i < monstersPerSpawn; i++)
        {
            Vector3 spawnPos = GetSpawnPositionNearPlayer();

            if (spawnPos != Vector3.zero)
            {
                GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                activeMonsters.Add(monster);

                // Make monster immediately aware of player
                ResonateAI ai = monster.GetComponent<ResonateAI>();
                if (ai != null)
                {
                    ai.HearNoise(player.position, 10f);
                }

                Debug.Log($"Monster spawned at {spawnPos} due to noise. Total active: {activeMonsters.Count}");
            }
        }
    }

    public void BlockSpawning(float duration)
    {
        spawnBlocked = true;
        spawnBlockEndTime = Time.time + duration;
        Debug.Log($"Spawning blocked for {duration} seconds");
    }

    Vector3 GetSpawnPositionNearPlayer()
    {
        int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * distance,
                0,
                Mathf.Sin(angle) * distance
            );

            Vector3 spawnPos = player.position + offset;

            // Raycast down to find ground
            RaycastHit hit;
            if (Physics.Raycast(spawnPos + Vector3.up * 50f, Vector3.down, out hit, 100f, groundLayer))
            {
                return hit.point + Vector3.up * 0.5f;
            }
        }

        Debug.LogWarning("Could not find valid spawn position near player");
        return Vector3.zero;
    }

    void CheckForDespawn()
    {
        List<GameObject> toDespawn = new List<GameObject>();

        foreach (GameObject monster in activeMonsters)
        {
            if (monster == null) continue;

            float distance = Vector3.Distance(player.position, monster.transform.position);

            if (distance > despawnDistance)
            {
                toDespawn.Add(monster);
            }
        }

        foreach (GameObject monster in toDespawn)
        {
            activeMonsters.Remove(monster);
            Destroy(monster);
            Debug.Log($"Monster despawned (too far). Remaining: {activeMonsters.Count}");
        }
    }

    public int GetActiveMonsterCount()
    {
        return activeMonsters.Count;
    }

    public void DespawnAllMonsters()
    {
        foreach (GameObject monster in activeMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        activeMonsters.Clear();
        Debug.Log("All monsters despawned");
    }
}