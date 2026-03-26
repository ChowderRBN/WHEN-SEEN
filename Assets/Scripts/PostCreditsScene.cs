using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostCreditsScene : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;
    public Transform cameraStartPosition;
    public Transform cameraEndPosition;
    public float cameraMoveSpeed = 2f;

    [Header("Audio Log")]
    public AudioSource voiceSource;
    public AudioClip[] logClips;

    [Header("UI")]
    public Image fadePanel;
    public GameObject subtitlePanel;
    public TextMeshProUGUI subtitleText;
    public Image staticEffect;

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
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }

        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);

        if (staticEffect != null)
            staticEffect.gameObject.SetActive(false);

        StartCoroutine(PlayPostCreditsSequence());
    }

    void Update()
    {
        if (isPlaying && mainCamera != null && cameraEndPosition != null)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                cameraEndPosition.position,
                Time.deltaTime * cameraMoveSpeed
            );
        }

        if (isPlaying && Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StopAllCoroutines();
            ReturnToMenu();
        }
    }

    IEnumerator PlayPostCreditsSequence()
    {
        yield return new WaitForSeconds(initialDelay);

        yield return StartCoroutine(FadeFromBlack());

        isPlaying = true;

        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(PlayAudioLog());

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(FadeToBlack());

        ReturnToMenu();
    }

    IEnumerator PlayAudioLog()
    {
        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);

        for (int i = 0; i < logClips.Length; i++)
        {
            if (logClips[i] != null && voiceSource != null)
            {
                if (subtitleText != null && i < subtitles.Length)
                    subtitleText.text = subtitles[i];

                voiceSource.PlayOneShot(logClips[i]);

                yield return new WaitForSeconds(logClips[i].length);
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return StartCoroutine(StaticCutoff());
    }

    IEnumerator StaticCutoff()
    {
        if (staticEffect != null)
            staticEffect.gameObject.SetActive(true);

        if (subtitleText != null)
            subtitleText.text = "█████ ██████ ███ ████████";

        for (int i = 0; i < 5; i++)
        {
            if (staticEffect != null)
                staticEffect.enabled = !staticEffect.enabled;

            yield return new WaitForSeconds(0.1f);
        }

        if (subtitleText != null)
            subtitleText.text = "[SIGNAL LOST]";

        yield return new WaitForSeconds(2f);

        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
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