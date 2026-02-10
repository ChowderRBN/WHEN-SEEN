using UnityEngine;

public class ScrollingCredits : MonoBehaviour
{
    public RectTransform creditsTransform;
    public GameObject creditsPanel;     // Panel that holds the credits
    public GameObject mainMenuPanel;    // Main menu panel to re-enable
    public float scrollSpeed = 50f;
    public float endYPosition = 800f;   // Position where credits end
    public float startYPosition = -500f; // Starting position (off-screen at bottom)

    void OnEnable()
    {
        // Reset credits position when panel is enabled
        ResetCredits();
    }

    void Update()
    {
        creditsTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditsTransform.anchoredPosition.y >= endYPosition)
        {
            EndCredits();
        }

        // Optional: Press ESC to go back
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMainMenu();
        }
    }

    void EndCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void ResetCredits()
    {
        // Reset to starting position (off-screen)
        creditsTransform.anchoredPosition = new Vector2(
            creditsTransform.anchoredPosition.x,
            startYPosition
        );
    }
}