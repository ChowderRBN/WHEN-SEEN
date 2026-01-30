using UnityEngine;
using System.Collections;

public class EchoformAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;       // How fast it moves toward the player
    public float stoppingDistance = 0.5f; // How close it gets before stopping

    [Header("Player Reference")]
    public Transform player;

    [Header("Visibility")]
    public Renderer rend;              // Assign the mesh renderer (optional)
    public float fadeSpeed = 3f;       // How fast it fades in/out
    private float targetAlpha = 0f;    // Current target alpha (0 = invisible, 1 = fully visible)
    private Material mat;

    void Start()
    {
        // Auto-find player if not assigned
        if (player == null && GameObject.FindWithTag("Player") != null)
            player = GameObject.FindWithTag("Player").transform;

        // Get material for fading
        if (rend != null)
            mat = rend.material;

        // Start invisible
        if (mat != null)
        {
            Color c = mat.color;
            c.a = 0f;
            mat.color = c;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Always chase the player
        Vector3 dir = player.position - transform.position;
        dir.y = 0f; // Lock movement to horizontal plane

        if (dir.magnitude > stoppingDistance)
            transform.position += dir.normalized * moveSpeed * Time.deltaTime;

        // Smoothly fade material
        if (mat != null)
        {
            Color c = mat.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            mat.color = c;
        }
    }

    /// <summary>
    /// Called by the echolocation to reveal this enemy
    /// </summary>
    public void Reveal(float strength = 1f)
    {
        targetAlpha = Mathf.Clamp01(strength); // Make it visible based on distance strength
    }

    /// <summary>
    /// Optional: hide again
    /// </summary>
    public void Hide()
    {
        targetAlpha = 0f;
    }
}
