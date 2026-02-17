using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DemoWinSequence : MonoBehaviour
{
    [Header("Win Settings")]
    public int coresNeededToWin = 2;
    public NulliumInventory inventory;

    [Header("UI")]
    public GameObject winPanel;
    public RectTransform creditsContainer; // Container that scrolls
    public RectTransform viewport;         // ADD: viewport for bounds checking
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI thankYouText;
    public TextMeshProUGUI skipText;
    public RawImage fadePanel;

    [Header("Scrolling Settings")]
    public float scrollSpeed = 50f;
    public float creditsStartY = -400f;

    [Header("Credits")]
    [TextArea(15, 30)]
    public string creditsMessage = @"


DEMO COMPLETE


CREDITS

Game Design & Programming
Your Name

Art & Design
Artist Name

Audio & Sound Design
Sound Designer Name

Music
Composer Name

Special Thanks
Your Friends
Your Team
Your Family

Created with Unity

Thank you for playing!
";

    [Header("Thank You Message")]
    [TextArea(5, 10)]
    public string thankYouMessage = @"THANK YOU FOR PLAYING!

This is a DEMO version of the game.

The full version will include:
- More areas to explore
- Additional abilities
- Multiple endings
- Enhanced AI
- And much more!

Stay tuned for the full release!";

    [Header("Timing")]
    public float fadeInDuration = 1.5f;
    public float fadeOutDuration = 1.5f;
    public float thankYouDisplayTime = 6f;

    [Header("Next Scene")]
    public string menuSceneName = "Menu";

    private bool hasWon = false;
    private bool canSkip = false;
    private bool isScrolling = false;

    void Start()
    {
        if (inventory == null)
            inventory = FindObjectOfType<NulliumInventory>();

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Update()
    {
        // Win condition
        if (!hasWon && inventory != null && inventory.GetCoreCount() >= coresNeededToWin)
        {
            hasWon = true;
            StartCoroutine(PlayWinSequence());
        }

        // Scroll credits safely
        if (isScrolling && creditsContainer != null)
        {
            creditsContainer.anchoredPosition +=
                Vector2.up * scrollSpeed * Time.deltaTime;

            // Reliable end check (no magic numbers)
            if (IsCreditsOffScreen())
            {
                Debug.Log("Credits finished scrolling");
                isScrolling = false;
                StartCoroutine(TransitionToThankYou());
            }
        }

        // Skip
        if (canSkip && Input.anyKeyDown)
        {
            StopAllCoroutines();
            isScrolling = false;
            ReturnToMenu();
        }
    }

    bool IsCreditsOffScreen()
    {
        if (viewport == null || creditsContainer == null)
            return false;

        float creditsBottom =
            creditsContainer.anchoredPosition.y +
            creditsContainer.rect.height;

        return creditsBottom >= viewport.rect.height;
    }

    IEnumerator PlayWinSequence()
    {
        Debug.Log("DEMO COMPLETED");

        canSkip = false;

        yield return StartCoroutine(FadeToBlack());

        if (winPanel != null)
            winPanel.SetActive(true);

        if (creditsText != null)
        {
            creditsText.gameObject.SetActive(true);
            creditsText.text = creditsMessage;
        }

        if (thankYouText != null)
            thankYouText.gameObject.SetActive(false);

        if (skipText != null)
            skipText.text = "Press any key to skip";

        // Preserve X (important)
        if (creditsContainer != null)
        {
            creditsContainer.anchoredPosition =
                new Vector2(
                    creditsContainer.anchoredPosition.x,
                    creditsStartY
                );
        }

        yield return StartCoroutine(FadeFromBlack());

        canSkip = true;
        isScrolling = true;
    }

    IEnumerator TransitionToThankYou()
    {
        Debug.Log("Transitioning to THANK YOU");

        yield return StartCoroutine(FadeToBlack());

        if (creditsText != null)
            creditsText.gameObject.SetActive(false);

        if (thankYouText != null)
        {
            thankYouText.gameObject.SetActive(true);
            thankYouText.text = thankYouMessage;
        }

        yield return StartCoroutine(FadeFromBlack());

        yield return new WaitForSeconds(thankYouDisplayTime);

        ReturnToMenu();
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1;
        fadePanel.color = color;
    }

    IEnumerator FadeFromBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = 1;
        fadePanel.color = color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsed / fadeOutDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0;
        fadePanel.color = color;
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
