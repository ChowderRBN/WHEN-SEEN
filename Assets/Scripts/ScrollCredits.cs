using UnityEngine;
using UnityEngine.UI;

public class ScrollCredits : MonoBehaviour
{
    public float scrollSpeed = 40f; // Speed at which the credits scroll

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
    }
}