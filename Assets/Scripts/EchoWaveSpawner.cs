using UnityEngine;
using System.Collections;

public class EchoWaveSpawner : MonoBehaviour
{
    [Header("Wave Prefab")]
    public GameObject echoWavePrefab;

    [Header("Ground Detection")]
    public float groundCheckDistance = 3f;
    public float forwardOffset = 0.6f;

    public IEnumerator SpawnEchoWave(float maxRadius, GameObject wavePrefab, float duration, float speed)
    {
        if (wavePrefab == null) yield break;

        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance))
            yield break;

        Vector3 origin = hit.point + transform.forward * forwardOffset + Vector3.up * 0.05f;

        GameObject wave = Instantiate(wavePrefab, origin, Quaternion.identity);
        wave.transform.localScale = Vector3.zero;

        float radius = 0f;
        float elapsed = 0f;

        EchoReveal[] revealables = FindObjectsOfType<EchoReveal>();

        while (elapsed < duration)
        {
            radius += speed * Time.deltaTime;
            float r = Mathf.Min(radius, maxRadius);

            wave.transform.localScale = new Vector3(r, 0.05f, r);

            foreach (EchoReveal reveal in revealables)
            {
                if (reveal == null) continue;

                float dist = Vector3.Distance(origin, reveal.transform.position);
                if (dist <= r)
                {
                    float strength = Mathf.InverseLerp(r, 0f, dist);
                    reveal.Reveal(strength);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(wave);
    }
}
