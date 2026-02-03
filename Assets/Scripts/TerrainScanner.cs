using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScanner : MonoBehaviour
{
    public GameObject TerrainScannerPrefab;
    public float duration = 10;
    public float size = 500;
    public float expansionSpeed = 50f; // Add this parameter to control expansion speed

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnTerrainScanner();
        }
    }

    public void SpawnTerrainScanner()
    {
        GameObject terrainScanner = Instantiate(TerrainScannerPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        ParticleSystem terrainScannerPS = terrainScanner.transform.GetChild(0).GetComponent<ParticleSystem>();

        if (terrainScannerPS != null)
        {
            var main = terrainScannerPS.main;
            main.startLifetime = duration;
            main.startSize = size;

            // Control expansion speed using Size Over Lifetime
            var sizeOverLifetime = terrainScannerPS.sizeOverLifetime;
            sizeOverLifetime.enabled = true;

            // Create a curve that controls how fast the particle grows
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0f); // Start at 0% size
            curve.AddKey(1f, 1f); // End at 100% size

            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(expansionSpeed, curve);
        }
        else
        {
            Debug.LogWarning("The first child of the TerrainScannerPrefab does not have a Particle System component.");
        }

        Destroy(terrainScanner, duration + 1); // Destroy after effect duration
    }
}