using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject cutscenePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public Image fadePanel;
    public Image flashEffect; // Optional: for screen flash effects

    [Header("Audio Sources")]
    public AudioSource voiceSource; // For voice lines
    public AudioSource sfxSource; // For sound effects
    public AudioSource musicSource; // For background music/ambience

    [Header("Background Audio")]
    public AudioClip ambientDarkness; // Removed alarmSound

    [Header("Cutscene Settings")]
    public float textSpeed = 0.02f; // Changed default to faster
    public string nextSceneName = "GameScene";

    [Header("Dialogue Lines")]
    public DialogueLine[] dialogueLines;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool skipTyping = false;

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // Fade in from black
        yield return StartCoroutine(FadeFromBlack(1f));

        // Start ambient darkness sound immediately (optional)
        if (ambientDarkness != null && musicSource != null)
        {
            musicSource.loop = true;
            musicSource.clip = ambientDarkness;
            musicSource.volume = 0.3f;
            musicSource.Play();
        }

        // Start dialogue IMMEDIATELY after fade
        yield return StartCoroutine(PlayDialogue());

        // Fade to black
        yield return StartCoroutine(FadeToBlack(2f));

        // Load game scene
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator PlayDialogue()
    {
        for (currentLineIndex = 0; currentLineIndex < dialogueLines.Length; currentLineIndex++)
        {
            DialogueLine line = dialogueLines[currentLineIndex];

            // Play voice line if assigned
            if (line.voiceClip != null && voiceSource != null)
            {
                voiceSource.PlayOneShot(line.voiceClip);
            }

            // Play sound effect if assigned
            if (line.soundEffect != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(line.soundEffect);
            }

            // Special visual effects based on line type
            if (line.useScreenShake)
            {
                StartCoroutine(ScreenShake(line.shakeIntensity, line.shakeDuration));
            }

            if (line.useRedFlash)
            {
                StartCoroutine(RedFlash(line.flashIntensity));
            }

            // Set speaker name
            if (speakerNameText != null)
            {
                speakerNameText.text = line.speakerName;
            }

            // Type out dialogue
            yield return StartCoroutine(TypeText(line.dialogueText, line.customTextSpeed > 0 ? line.customTextSpeed : textSpeed));

            // Wait for input or auto-advance
            if (line.autoAdvance)
            {
                yield return new WaitForSeconds(line.autoAdvanceDelay);
            }
            else
            {
                yield return StartCoroutine(WaitForInput());
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator TypeText(string text, float speed)
    {
        isTyping = true;
        skipTyping = false;
        dialogueText.text = "";

        foreach (char letter in text)
        {
            if (skipTyping)
            {
                dialogueText.text = text;
                break;
            }

            dialogueText.text += letter;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
    }

    IEnumerator WaitForInput()
    {
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Return) && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
    }

    void Update()
    {
        // Skip typing animation
        if (isTyping && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            skipTyping = true;
        }

        // Press ESC to skip entire cutscene
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator FadeToBlack(float duration)
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / duration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1;
        fadePanel.color = color;
    }

    IEnumerator FadeFromBlack(float duration)
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 1;
        fadePanel.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsed / duration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0;
        fadePanel.color = color;
    }

    IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        if (source == null) yield break;

        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    IEnumerator RedFlash(float intensity)
    {
        if (flashEffect == null) yield break;

        Color red = new Color(1, 0, 0, intensity);
        flashEffect.color = red;

        yield return new WaitForSeconds(0.1f);

        red.a = 0;
        flashEffect.color = red;
    }

    IEnumerator ScreenShake(float intensity, float duration)
    {
        if (cutscenePanel == null) yield break;

        Vector3 originalPos = cutscenePanel.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);

            cutscenePanel.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cutscenePanel.transform.localPosition = originalPos;
    }
}

// Enhanced Dialogue Line class with audio options
[System.Serializable]
public class DialogueLine
{
    [Header("Text")]
    public string speakerName;
    [TextArea(3, 5)]
    public string dialogueText;

    [Header("Audio")]
    public AudioClip voiceClip; // Voice acting for this line
    public AudioClip soundEffect; // Sound effect to play with this line

    [Header("Timing")]
    public float customTextSpeed = 0; // Leave at 0 to use default speed
    public bool autoAdvance = false; // If true, doesn't wait for player input
    public float autoAdvanceDelay = 2f; // How long to wait before auto-advancing

    [Header("Visual Effects")]
    public bool useScreenShake = false;
    public float shakeIntensity = 10f;
    public float shakeDuration = 0.5f;

    public bool useRedFlash = false;
    public float flashIntensity = 0.5f;
}