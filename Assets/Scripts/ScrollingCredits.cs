using UnityEngine;
using UnityEngine.InputSystem;

public class ScrollingCredits : MonoBehaviour
{
    public RectTransform creditsTransform;
    public GameObject creditsPanel;
    public GameObject mainMenuPanel;
    public float scrollSpeed = 50f;
    public float endYPosition = 800f;
    public float startYPosition = -500f;

    void OnEnable()
    {
        ResetCredits();
    }

    void Update()
    {
        creditsTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditsTransform.anchoredPosition.y >= endYPosition)
        {
            EndCredits();
        }

        // New Input System replacement for Input.GetKeyDown(KeyCode.Escape)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
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
        creditsTransform.anchoredPosition = new Vector2(
            creditsTransform.anchoredPosition.x,
            startYPosition
        );
    }
}