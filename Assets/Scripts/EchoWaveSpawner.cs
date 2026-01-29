using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EchoWaveSpawner : MonoBehaviour
{
    [Header("Wave Prefabs")]
    public GameObject echoWavePrefab;
    public GameObject miniStepWavePrefab;

    [Header("Ground Detection")]
    public LayerMask groundMask;
    public float groundCheckDistance = 3f;
    public float forwardOffset = 0.6f;

    [Header("Monsters")]
    public List<ResonateAI> resonates = new();
    public List<EchoformAI> echoforms = new();

    public IEnumerator SpawnEchoWave(
        float maxRadius,
        GameObject wavePrefab,
        float duration,
        float speed
    )
    {
        if (wavePrefab == null)
            yield break;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.2f;
        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
            yield break;

        Vector3 origin = hit.point + transform.forward * forwardOffset + Vector3.up * 0.05f;

        GameObject wave = Instantiate(wavePrefab, origin, Quaternion.identity);
        wave.transform.localScale = Vector3.zero;

        float radius = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (wave == null) yield break;

            radius += speed * Time.deltaTime;
            float r = Mathf.Min(radius, maxRadius);
            wave.transform.localScale = new Vector3(r, 0.2f, r);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(wave);

        // Alert monsters
        for (int i = resonates.Count - 1; i >= 0; i--)
        {
            if (resonates[i] == null) resonates.RemoveAt(i);
            else resonates[i].Alert(origin, maxRadius);
        }

        for (int i = echoforms.Count - 1; i >= 0; i--)
        {
            if (echoforms[i] == null) echoforms.RemoveAt(i);
            else echoforms[i].Reveal();
        }
    }
}
