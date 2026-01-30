using UnityEngine;
using System.Collections;

public class PlayerManualEcholocation : MonoBehaviour
{
    public EchoWaveSpawner spawner;
    public int maxPingCharges = 3;
    public float pingRechargeTime = 25f;
    public float radius = 12f;
    public float duration = 1f;
    public float speed = 6f;

    int charges;
    bool canPing = true;

    void Start() => charges = maxPingCharges;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && charges > 0 && canPing)
        {
            StartCoroutine(spawner.SpawnEchoWave(radius, spawner.echoWavePrefab, duration, speed));
            charges--;
            StartCoroutine(Recharge());
        }
    }

    IEnumerator Recharge()
    {
        canPing = false;
        yield return new WaitForSeconds(pingRechargeTime);
        charges = Mathf.Min(charges + 1, maxPingCharges);
        canPing = true;
    }
}
