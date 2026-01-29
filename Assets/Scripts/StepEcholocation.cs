using UnityEngine;
using System.Collections;

public class PlayerStepEcholocation : MonoBehaviour
{
    public FirstPersonController fpController;
    public EchoWaveSpawner spawner;

    public float stepDistance = 0.25f;
    public float radius = 1.2f;
    public float duration = 0.5f;
    public float speed = 4f;

    Vector3 lastStepPos;

    void Start()
    {
        lastStepPos = transform.position;
    }

    void Update()
    {
        if (fpController.IsCrouching)
            return;

        float moved = Vector3.Distance(transform.position, lastStepPos);
        if (moved >= stepDistance)
        {
            StartCoroutine(spawner.SpawnEchoWave(
                radius,
                spawner.miniStepWavePrefab,
                duration,
                speed
            ));

            lastStepPos = transform.position;
        }
    }
}
