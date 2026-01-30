using UnityEngine;
using System.Collections;

public class EchoReveal : MonoBehaviour
{
    public Renderer rend;
    public float visibleTime = 1.5f;

    Coroutine revealRoutine;

    void Awake()
    {
        if (rend == null)
            rend = GetComponentInChildren<Renderer>();

        SetVisible(false);
    }

    public void Reveal(float strength)
    {
        if (revealRoutine != null)
            StopCoroutine(revealRoutine);

        revealRoutine = StartCoroutine(RevealRoutine(strength));
    }

    IEnumerator RevealRoutine(float strength)
    {
        SetVisible(true);

        // Emission strength (Daredevil glow)
        Color glow = Color.white * Mathf.Lerp(0.3f, 2.5f, strength);
        rend.material.SetColor("_EmissionColor", glow);

        yield return new WaitForSeconds(visibleTime);

        SetVisible(false);
    }

    void SetVisible(bool state)
    {
        rend.enabled = state;
    }
}
