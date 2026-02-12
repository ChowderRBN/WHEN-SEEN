using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CaveTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform exitPoint; // Where the player exits

    [Header("Fade Settings")]
    public Image fadeImage; // Black screen overlay
    public float fadeInDuration = 2f; // Time to fade to black
    public float fadeOutDuration = 2f; // Time to fade from black
    public float walkThroughDelay = 1f; // Time spent in darkness (simulating walking)

    [Header("Player")]
    public FirstPersonController playerController;
    public GameObject player;

    private bool isTeleporting = false;
    private Canvas fadeCanvas;

    void Start()
    {
        // Setup fade image
        if (fadeImage != null)
        {
            // Make sure the canvas is set to screen space overlay
            fadeCanvas = fadeImage.GetComponentInParent<Canvas>();
            if (fadeCanvas != null)
            {
                fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                fadeCanvas.sortingOrder = 9999; // Render on top of everything
            }

            // Ensure fade panel covers the whole screen
            RectTransform rect = fadeImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Start transparent
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false; // Don't block UI clicks when invisible
            fadeImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Fade Image not assigned to CaveTeleport!");
        }

        // Auto-find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerController == null && player != null)
        {
            playerController = player.GetComponent<FirstPersonController>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            Debug.Log("Cave teleport triggered!");
            StartCoroutine(TeleportPlayer());
        }
    }

    IEnumerator TeleportPlayer()
    {
        isTeleporting = true;

        // Make sure fade image is active and visible
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
        }

        // Disable player control during teleport
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Fade to black
        Debug.Log("Fading to black...");
        yield return StartCoroutine(FadeToBlack());

        // Wait in darkness (simulating walking through the cave)
        yield return new WaitForSeconds(walkThroughDelay);

        // Teleport player to exit
        if (player != null && exitPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                player.transform.position = exitPoint.position;
                player.transform.rotation = exitPoint.rotation;
                cc.enabled = true;
            }
            else
            {
                player.transform.position = exitPoint.position;
                player.transform.rotation = exitPoint.rotation;
            }
            Debug.Log("Player teleported!");
        }

        // Fade from black
        Debug.Log("Fading from black...");
        yield return StartCoroutine(FadeFromBlack());

        // Re-enable player control
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isTeleporting = false;
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;
        Debug.Log("Fade to black complete");
    }

    IEnumerator FadeFromBlack()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 1;
        fadeImage.color = color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, elapsed / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
        Debug.Log("Fade from black complete");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.5f);

        if (exitPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(exitPoint.position, 1.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, exitPoint.position);
        }
    }
}