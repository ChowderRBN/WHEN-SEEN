using UnityEngine;
using System.Collections;

public class ResonateAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public Transform target; // player

    private bool alerted = false;
    private bool fleeing = false;
    private Vector3 fleeDirection;

    void Update()
    {
        if (alerted && !fleeing && target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }

        if (fleeing)
        {
            transform.position += fleeDirection * moveSpeed * Time.deltaTime;
        }
    }

    public void Alert(Vector3 waveOrigin, float waveRadius)
    {
        float dist = Vector3.Distance(transform.position, waveOrigin);
        if (dist <= waveRadius)
        {
            alerted = true;
            fleeing = false;
        }
    }

    public void Flee(Vector3 screamOrigin)
    {
        fleeing = true;
        alerted = false;
        fleeDirection = (transform.position - screamOrigin).normalized;
        StartCoroutine(CalmDownAfterSeconds(5f));
    }

    private IEnumerator CalmDownAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        fleeing = false;
        alerted = false;
    }
}
