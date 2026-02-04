using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollingCredits : MonoBehaviour
{
    public RectTransform creditsTransform;
    public float scrollSpeed = 50f;
    public float endYPosition = 800f;

    void Update()
    {
        creditsTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (creditsTransform.anchoredPosition.y >= endYPosition)
        {
            EndCredits();
        }
    }

    void EndCredits()
    {
        SceneManager.LoadScene("Menu");
    }
}
