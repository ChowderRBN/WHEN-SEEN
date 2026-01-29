using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAdrenaline : MonoBehaviour
{
    public FirstPersonController controller;
    public EchoWaveSpawner spawner;

    public float triggerDistance = 5f;
    public float duration = 10f;
    public float speedMultiplier = 1.65f;

    public List<ResonateAI> resonates = new();

    bool active;

    void Update()
    {
        if (active) return;

        foreach (var r in resonates)
        {
            if (r == null) continue;

            if (Vector3.Distance(transform.position, r.transform.position) <= triggerDistance)
            {
                StartCoroutine(Activate());
                break;
            }
        }
    }

    IEnumerator Activate()
    {
        active = true;

        StartCoroutine(spawner.SpawnEchoWave(
            15f,
            spawner.echoWavePrefab,
            1f,
            6f
        ));

        controller.SetSpeedMultiplier(speedMultiplier);

        yield return new WaitForSeconds(duration);

        controller.ResetSpeed();
        active = false;
    }
}
