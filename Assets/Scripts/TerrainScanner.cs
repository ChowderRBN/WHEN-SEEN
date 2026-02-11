using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScanner : MonoBehaviour
{
    public GameObject TerrainScannerPrefab;
    public float duration = 10;
    public float size = 500;

    [Header("Noise Detection")]
    public NoiseDetection noiseDetection;

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
        }
        else
        {
            Debug.LogWarning("The first child of the TerrainScannerPrefab does not have a Particle System component.");
        }

        // Notify noise system that echolocation was used
        if (noiseDetection != null)
        {
            noiseDetection.NotifyEcholocationUsed();
        }

        Destroy(terrainScanner, duration + 1);
    }
}