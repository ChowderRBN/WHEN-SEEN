using UnityEngine;

public class EchoEmitter : MonoBehaviour
{
    public GameObject echoDomePrefab;
    public float spawnHeightOffset = 0.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EmitEcho();
        }
    }

    void EmitEcho()
    {
        if (echoDomePrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * spawnHeightOffset;
        Instantiate(echoDomePrefab, spawnPos, Quaternion.identity);
    }
}
