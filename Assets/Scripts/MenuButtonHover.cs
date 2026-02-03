using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Effect")]
    public bool useScaleEffect = true;
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;

    [Header("Color Effect")]
    public bool useColorEffect = true;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    [Header("References")]
    public MenuNavigator menuNavigator;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Image buttonImage;
    private Text buttonText;
    private Color targetColor;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<Text>();

        if (buttonImage != null)
        {
            normalColor = buttonImage.color;
            targetColor = normalColor;
        }
        else if (buttonText != null)
        {
            normalColor = buttonText.color;
            targetColor = normalColor;
        }

        // Find MenuNavigator if not assigned
        if (menuNavigator == null)
        {
            menuNavigator = FindObjectOfType<MenuNavigator>();
        }
    }

    void Update()
    {
        // Smooth scale transition
        if (useScaleEffect)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        // Smooth color transition
        if (useColorEffect)
        {
            if (buttonImage != null)
            {
                buttonImage.color = Color.Lerp(buttonImage.color, targetColor, Time.deltaTime * scaleSpeed);
            }
            else if (buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, targetColor, Time.deltaTime * scaleSpeed);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;

        if (menuNavigator != null)
        {
            menuNavigator.PlayButtonHover();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        targetColor = normalColor;
    }
}