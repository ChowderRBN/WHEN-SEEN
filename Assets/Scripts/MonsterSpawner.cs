using UnityEngine;
using System.Collections.Generic;

public class AdvancedMonsterSpawner : MonoBehaviour
{
    [Header("Monster Settings")]
    public GameObject monsterPrefab;
    public int minMonsters = 5;
    public int maxMonsters = 15;

    [Header("Spawn Zones")]
    public Transform[] spawnZones; // Assign empty GameObjects around your map
    public float spawnZoneRadius = 30f;

    [Header("Spawn Rules")]
    public float minDistanceFromPlayer = 20f;
    public float minDistanceBetweenMonsters = 10f;
    public LayerMask groundLayer;

    [Header("Player")]
    public Transform player;

    private List<GameObject> spawnedMonsters = new List<GameObject>();

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        SpawnMonsters();
    }

    public void SpawnMonsters()
    {
        int monsterCount = Random.Range(minMonsters, maxMonsters + 1);

        for (int i = 0; i < monsterCount; i++)
        {
            // Pick random spawn zone
            Transform zone = spawnZones[Random.Range(0, spawnZones.Length)];

            // Try to spawn in that zone
            Vector3 spawnPos = GetRandomPositionInZone(zone);

            if (IsValidSpawnPosition(spawnPos))
            {
                GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                spawnedMonsters.Add(monster);
            }
        }

        Debug.Log($"Spawned {spawnedMonsters.Count} monsters across {spawnZones.Length} zones");
    }

    Vector3 GetRandomPositionInZone(Transform zone)
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnZoneRadius;
        Vector3 randomPos = zone.position + new Vector3(randomCircle.x, 50f, randomCircle.y);

        RaycastHit hit;
        if (Physics.Raycast(randomPos, Vector3.down, out hit, 100f, groundLayer))
        {
            return hit.point + Vector3.up * 0.5f;
        }

        return zone.position;
    }

    bool IsValidSpawnPosition(Vector3 position)
    {
        // Check player distance
        if (player != null && Vector3.Distance(position, player.position) < minDistanceFromPlayer)
        {
            return false;
        }

        // Check other monsters
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null && Vector3.Distance(position, monster.transform.position) < minDistanceBetweenMonsters)
            {
                return false;
            }
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnZones == null) return;

        Gizmos.color = Color.cyan;
        foreach (Transform zone in spawnZones)
        {
            if (zone != null)
            {
                Gizmos.DrawWireSphere(zone.position, spawnZoneRadius);
            }
        }
    }
}