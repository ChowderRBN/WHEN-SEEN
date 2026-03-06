using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject cutscenePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public Image fadePanel;
    public Image flashEffect;

    [Header("Name Input UI")]
    public GameObject nameInputPanel;
    public TMP_InputField nameInputField;
    public GameObject confirmNameButton;
    public TextMeshProUGUI namePromptText;

    [Header("Audio Sources")]
    public AudioSource voiceSource;
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Background Audio")]
    public AudioClip ambientDarkness;

    [Header("Cutscene Settings")]
    public float textSpeed = 0.02f;
    public string nextSceneName = "Game2";
    public string defaultPlayerName = "Survivor";

    [Header("Dialogue Lines")]
    public DialogueLine[] dialogueLines;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool skipTyping = false;
    private string playerName = "";
    private bool nameConfirmed = false;

    void Start()
    {
        if (cutscenePanel != null)
            cutscenePanel.SetActive(false);

        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 1;
            fadePanel.color = color;
        }

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        yield return StartCoroutine(GetPlayerName());

        if (cutscenePanel != null)
            cutscenePanel.SetActive(true);

        yield return StartCoroutine(FadeFromBlack(1f));

        if (ambientDarkness != null && musicSource != null)
        {
            musicSource.loop = true;
            musicSource.clip = ambientDarkness;
            musicSource.volume = 0.3f;
            musicSource.Play();
        }

        yield return StartCoroutine(PlayDialogue());
        yield return StartCoroutine(FadeToBlack(2f));

        if (GameManager.Instance != null)
            GameManager.Instance.playerName = playerName;
        else
            PlayerPrefs.SetString("PlayerName", playerName);

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator GetPlayerName()
    {
        Debug.Log("Showing name input panel...");

        if (nameInputPanel != null)
            nameInputPanel.SetActive(true);

        if (namePromptText != null)
            namePromptText.text = "ENTER YOUR NAME";

        if (nameInputField != null)
        {
            nameInputField.text = "";
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }

        nameConfirmed = false;

        while (!nameConfirmed)
        {
            // New Input System: check Enter or Numpad Enter
            bool enterPressed = Keyboard.current != null &&
                (Keyboard.current.enterKey.wasPressedThisFrame ||
                 Keyboard.current.numpadEnterKey.wasPressedThisFrame);

            if (enterPressed && ValidateName())
            {
                nameConfirmed = true;
                Debug.Log("Name confirmed via Enter key!");
            }
            yield return null;
        }

        playerName = (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text.Trim()))
            ? nameInputField.text.Trim()
            : defaultPlayerName;

        Debug.Log($"Player name set to: {playerName}");

        if (nameInputPanel != null)
            nameInputPanel.SetActive(false);
    }

    public void OnConfirmNameClicked()
    {
        Debug.Log("Confirm button clicked!");

        if (ValidateName())
        {
            nameConfirmed = true;
            Debug.Log("Name is valid, confirming...");
        }
        else
        {
            Debug.Log("Name is invalid (empty)");
        }
    }

    bool ValidateName()
    {
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text.Trim()))
            return true;

        if (nameInputField != null)
            StartCoroutine(FlashInputField());

        return false;
    }

    IEnumerator FlashInputField()
    {
        Image inputImage = nameInputField.GetComponent<Image>();
        if (inputImage == null) yield break;

        Color originalColor = inputImage.color;
        Color flashColor = new Color(1f, 0.5f, 0.5f);

        for (int i = 0; i < 3; i++)
        {
            inputImage.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            inputImage.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator PlayDialogue()
    {
        for (currentLineIndex = 0; currentLineIndex < dialogueLines.Length; currentLineIndex++)
        {
            DialogueLine line = dialogueLines[currentLineIndex];

            if (line.voiceClip != null && voiceSource != null)
                voiceSource.PlayOneShot(line.voiceClip);

            if (line.soundEffect != null && sfxSource != null)
                sfxSource.PlayOneShot(line.soundEffect);

            if (line.useScreenShake)
                StartCoroutine(ScreenShake(line.shakeIntensity, line.shakeDuration));

            if (line.useRedFlash)
                StartCoroutine(RedFlash(line.flashIntensity));

            if (speakerNameText != null)
                speakerNameText.text = line.speakerName.Replace("{PLAYER}", playerName);

            string processedDialogue = line.dialogueText.Replace("{PLAYER}", playerName);
            yield return StartCoroutine(TypeText(processedDialogue, line.customTextSpeed > 0 ? line.customTextSpeed : textSpeed));

            if (line.autoAdvance)
                yield return new WaitForSeconds(line.autoAdvanceDelay);
            else
                yield return StartCoroutine(WaitForInput());

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
        while (true)
        {
            if (Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame ||
                 Keyboard.current.enterKey.wasPressedThisFrame))
                yield break;

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                yield break;

            yield return null;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Skip typing animation
        bool advancePressed = Keyboard.current.spaceKey.wasPressedThisFrame ||
                              Keyboard.current.enterKey.wasPressedThisFrame ||
                              (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (isTyping && advancePressed)
            skipTyping = true;

        // Skip cutscene entirely
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            StopAllCoroutines();

            if (string.IsNullOrEmpty(playerName))
                playerName = defaultPlayerName;

            if (GameManager.Instance != null)
                GameManager.Instance.playerName = playerName;
            else
                PlayerPrefs.SetString("PlayerName", playerName);

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

[System.Serializable]
public class DialogueLine
{
    [Header("Text")]
    public string speakerName;
    [TextArea(3, 5)]
    public string dialogueText;

    [Header("Audio")]
    public AudioClip voiceClip;
    public AudioClip soundEffect;

    [Header("Timing")]
    public float customTextSpeed = 0;
    public bool autoAdvance = false;
    public float autoAdvanceDelay = 2f;

    [Header("Visual Effects")]
    public bool useScreenShake = false;
    public float shakeIntensity = 10f;
    public float shakeDuration = 0.5f;

    public bool useRedFlash = false;
    public float flashIntensity = 0.5f;
}