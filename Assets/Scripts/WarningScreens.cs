using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningScreens : MonoBehaviour
{
    [Header("Warning Sprites")]
    public Sprite epilepsyWarningSprite;
    public Sprite headphoneWarningSprite;

    [Header("Sprite Renderer")]
    public SpriteRenderer spriteRenderer; // The renderer that will display the sprites

    [Header("Timing Settings")]
    public float epilepsyWarningDuration = 3f;
    public float headphoneWarningDuration = 3f;
    public float fadeDuration = 1f;

    [Header("Scene To Load")]
    public string menuSceneName = "Menu";

    [Header("Skip Option")]
    public bool allowSkip = true;

    void Start()
    {
        StartCoroutine(PlayWarnings());
    }

    void Update()
    {
        // Allow player to skip with any key
        if (allowSkip && Input.anyKeyDown)
        {
            StopAllCoroutines();
            LoadMenu();
        }
    }

    IEnumerator PlayWarnings()
    {
        // Fade in epilepsy warning
        spriteRenderer.sprite = epilepsyWarningSprite;
        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(epilepsyWarningDuration);

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Fade in headphone warning
        spriteRenderer.sprite = headphoneWarningSprite;
        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(headphoneWarningDuration);

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Load menu scene
        LoadMenu();
    }

    void LoadMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color color = spriteRenderer.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0;
        spriteRenderer.color = color;
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 1;
        spriteRenderer.color = color;
    }
}