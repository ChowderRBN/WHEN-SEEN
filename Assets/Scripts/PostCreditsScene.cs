using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PostCreditsScene : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;
    public Transform cameraStartPosition;
    public Transform cameraEndPosition;
    public float cameraMoveSpeed = 2f;

    [Header("Audio Log")]
    public AudioSource voiceSource;
    public AudioClip[] logClips; // Multiple voice lines

    [Header("UI")]
    public Image fadePanel;
    public GameObject subtitlePanel;
    public TextMeshProUGUI subtitleText;
    public Image staticEffect; // VHS static overlay

    [Header("Timing")]
    public float initialDelay = 2f;
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 2f;

    [Header("Next Scene")]
    public string menuSceneName = "Menu";

    [Header("Subtitles")]
    [TextArea(3, 10)]
    public string[] subtitles;

    private bool isPlaying = false;

    void Start()
    {
        // Setup fade panel
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 1f; // Start black
            fadePanel.color = c;
        }

        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }

        if (staticEffect != null)
        {
            staticEffect.gameObject.SetActive(false);
        }

        StartCoroutine(PlayPostCreditsSequence());
    }

    void Update()
    {
        // Slow camera dolly forward
        if (isPlaying && mainCamera != null && cameraEndPosition != null)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                cameraEndPosition.position,
                Time.deltaTime * cameraMoveSpeed
            );
        }

        // Skip with any key
        if (Input.anyKeyDown && isPlaying)
        {
            StopAllCoroutines();
            ReturnToMenu();
        }
    }

    IEnumerator PlayPostCreditsSequence()
    {
        // Initial black screen
        yield return new WaitForSeconds(initialDelay);

        // Fade in to reveal the massive planet
        yield return StartCoroutine(FadeFromBlack());

        isPlaying = true;

        // Wait a moment to let player take in the scale
        yield return new WaitForSeconds(3f);

        // Play audio log with subtitles
        yield return StartCoroutine(PlayAudioLog());

        // Wait a moment
        yield return new WaitForSeconds(2f);

        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Return to menu
        ReturnToMenu();
    }

    IEnumerator PlayAudioLog()
    {
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(true);
        }

        // Play each audio clip with subtitle
        for (int i = 0; i < logClips.Length; i++)
        {
            if (logClips[i] != null && voiceSource != null)
            {
                // Show subtitle
                if (subtitleText != null && i < subtitles.Length)
                {
                    subtitleText.text = subtitles[i];
                }

                // Play voice
                voiceSource.PlayOneShot(logClips[i]);

                // Wait for clip to finish
                yield return new WaitForSeconds(logClips[i].length);

                // Brief pause between lines
                yield return new WaitForSeconds(0.5f);
            }
        }

        // CRITICAL MOMENT - Audio cuts off dramatically
        yield return StartCoroutine(StaticCutoff());
    }

    IEnumerator StaticCutoff()
    {
        // Show static effect
        if (staticEffect != null)
        {
            staticEffect.gameObject.SetActive(true);
        }

        // Play static sound if you have one
        // voiceSource.PlayOneShot(staticSound);

        // Glitch the subtitle
        if (subtitleText != null)
        {
            subtitleText.text = "█████ ██████ ███ ████████";
        }

        // Flash static
        for (int i = 0; i < 5; i++)
        {
            if (staticEffect != null)
            {
                staticEffect.enabled = !staticEffect.enabled;
            }
            yield return new WaitForSeconds(0.1f);
        }

        // Signal lost
        if (subtitleText != null)
        {
            subtitleText.text = "[SIGNAL LOST]";
        }

        yield return new WaitForSeconds(2f);

        // Hide subtitles
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }
    }

    IEnumerator FadeFromBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 1f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        fadePanel.color = color;
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}