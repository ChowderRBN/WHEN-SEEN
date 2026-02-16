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
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI thankYouText;
    public TextMeshProUGUI skipText;
    public RawImage fadePanel;

    [Header("Scrolling Settings")]
    public float scrollSpeed = 50f; // Pixels per second
    public float creditsStartY = -400f; // Start below screen
    public float creditsEndY = 1200f; // End above screen

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
        // Auto-find inventory
        if (inventory == null)
        {
            inventory = FindObjectOfType<NulliumInventory>();
        }

        // Hide win panel
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Check for win condition
        if (!hasWon && inventory != null)
        {
            if (inventory.GetCoreCount() >= coresNeededToWin)
            {
                StartCoroutine(PlayWinSequence());
                hasWon = true;
            }
        }

        // Scroll credits
        if (isScrolling && creditsContainer != null)
        {
            creditsContainer.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            // Check if scrolling is done
            if (creditsContainer.anchoredPosition.y >= creditsEndY)
            {
                isScrolling = false;
                StartCoroutine(TransitionToThankYou());
            }
        }

        // Allow skipping
        if (canSkip && Input.anyKeyDown)
        {
            StopAllCoroutines();
            isScrolling = false;
            ReturnToMenu();
        }
    }

    IEnumerator PlayWinSequence()
    {
        Debug.Log("DEMO COMPLETED!");

        canSkip = false;

        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Show win panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Setup credits
        if (creditsText != null)
        {
            creditsText.gameObject.SetActive(true);
            creditsText.text = creditsMessage;
        }
        if (thankYouText != null)
        {
            thankYouText.gameObject.SetActive(false);
        }
        if (skipText != null)
        {
            skipText.text = "Press any key to skip";
        }

        // Reset credits position
        if (creditsContainer != null)
        {
            creditsContainer.anchoredPosition = new Vector2(0, creditsStartY);
        }

        // Fade from black
        yield return StartCoroutine(FadeFromBlack());

        canSkip = true;

        // Start scrolling
        isScrolling = true;
    }

    IEnumerator TransitionToThankYou()
    {
        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Hide credits
        if (creditsText != null)
        {
            creditsText.gameObject.SetActive(false);
        }

        // Show thank you
        if (thankYouText != null)
        {
            thankYouText.gameObject.SetActive(true);
            thankYouText.text = thankYouMessage;
        }

        // Fade from black
        yield return StartCoroutine(FadeFromBlack());

        // Display thank you
        yield return new WaitForSeconds(thankYouDisplayTime);

        // Return to menu
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