using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSonicScream : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;
    public AudioClip clip;

    [Header("Scream Settings")]
    public int maxScreams = 2;
    public float cooldown = 60f;
    public float fleeDistance = 50f; // How far enemies flee

    [Header("Noise Detection")]
    public NoiseDetection noiseDetection;

    [Header("Enemies")]
    public List<ResonateAI> resonates = new();

    private int screams;

    void Start()
    {
        screams = maxScreams;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && screams > 0)
        {
            // Play scream sound
            if (source != null && clip != null)
            {
                source.PlayOneShot(clip);
            }

            // Make all resonates flee
            foreach (var r in resonates)
            {
                if (r != null)
                {
                    r.Flee(transform.position, fleeDistance);
                }
            }

            // Notify noise detection system
            if (noiseDetection != null)
            {
                noiseDetection.NotifySonicScreamUsed();
            }

            screams--;
            StartCoroutine(Recharge());
        }
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(cooldown);
        screams = Mathf.Min(screams + 1, maxScreams);
    }
}