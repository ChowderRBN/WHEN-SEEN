using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRippleReveal : MonoBehaviour
{
    [Header("Terrain Scanner")]
    public TerrainScanner terrainScanner;

    [Header("Menu Elements")]
    public List<RectTransform> menuElements = new List<RectTransform>();

    [Header("Ripple Settings")]
    public Vector2 rippleOrigin = new Vector2(0.5f, 0.5f); // Center of screen (normalized)
    public float rippleSpeed = 500f;
    public float maxRippleDistance = 1000f;
    public float revealDelay = 0.1f; // Delay after terrain scanner starts

    private bool isRevealed = false;
    private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
    private Vector2 rippleWorldOrigin;

    void Start()
    {
        // Setup canvas groups for each menu element
        foreach (var element in menuElements)
        {
            CanvasGroup cg = element.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = element.gameObject.AddComponent<CanvasGroup>();
            }
            cg.alpha = 0f;
            canvasGroups.Add(cg);
        }

        // Calculate ripple origin in world space
        rippleWorldOrigin = new Vector2(
            Screen.width * rippleOrigin.x,
            Screen.height * rippleOrigin.y
        );
    }

    void Update()
    {
        if (!isRevealed && Input.GetKeyDown(KeyCode.E))
        {
            RevealMenu();
        }
    }

    public void RevealMenu()
    {
        if (isRevealed) return;

        // Trigger terrain scanner effect
        if (terrainScanner != null)
        {
            terrainScanner.SpawnTerrainScanner();
        }

        // Start ripple reveal
        StartCoroutine(RippleRevealCoroutine());

        isRevealed = true;
    }

    IEnumerator RippleRevealCoroutine()
    {
        // Optional delay to sync with terrain scanner
        yield return new WaitForSeconds(revealDelay);

        float currentRadius = 0f;

        while (currentRadius < maxRippleDistance)
        {
            currentRadius += rippleSpeed * Time.deltaTime;

            // Check each menu element
            for (int i = 0; i < menuElements.Count; i++)
            {
                if (canvasGroups[i].alpha < 1f)
                {
                    float distance = Vector2.Distance(rippleWorldOrigin, menuElements[i].position);

                    if (distance <= currentRadius)
                    {
                        // Calculate fade based on how long ago ripple passed
                        float fadeDuration = 0.5f;
                        float timeSinceHit = (currentRadius - distance) / rippleSpeed;
                        float fadeProgress = Mathf.Clamp01(timeSinceHit / fadeDuration);

                        canvasGroups[i].alpha = fadeProgress;
                    }
                }
            }

            yield return null;
        }

        // Ensure all elements are fully visible
        foreach (var cg in canvasGroups)
        {
            cg.alpha = 1f;
        }
    }
}