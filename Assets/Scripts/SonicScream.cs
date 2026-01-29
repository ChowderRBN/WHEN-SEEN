using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSonicScream : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;

    public int maxScreams = 2;
    public float cooldown = 60f;

    public List<ResonateAI> resonates = new();

    int screams;

    void Start()
    {
        screams = maxScreams;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && screams > 0)
        {
            source.PlayOneShot(clip);

            foreach (var r in resonates)
                if (r != null) r.Flee(transform.position);

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
