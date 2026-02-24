using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death Detection")]
    public float deathRange = 2f;

    [Header("Fade Settings")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMPro.TextMeshProUGUI gameOverText;
    public string deathMessage = "YOU WERE CONSUMED BY THE DARKNESS";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathScream;
    public AudioClip monsterAttack;

    [Header("Player")]
    public FirstPersonController playerController;

    private bool isDead = false;
    private SphereCollider deathTrigger;

    void Start()
    {
        // Setup fade panel
        if (fadePanel != null)
        {
            Canvas fadeCanvas = fadePanel.GetComponentInParent<Canvas>();
            if (fadeCanvas != null)
            {
                fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                fadeCanvas.sortingOrder = 9999;
            }

            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.gameObject.SetActive(true);
        }

        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Create death trigger collider
        deathTrigger = gameObject.AddComponent<SphereCollider>();
        deathTrigger.isTrigger = true;
        deathTrigger.radius = deathRange;

        if (playerController == null)
        {
            playerController = GetComponent<FirstPersonController>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it's a Resonate
        if (!isDead && other.GetComponent<ResonateAI>() != null)
        {
            StartCoroutine(Death());
        }
    }

    public void Kill()
    {
        if (!isDead)
        {
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        isDead = true;

        Debug.Log("Player killed by Resonate!");

        // Play death sounds
        if (audioSource != null)
        {
            if (monsterAttack != null)
            {
                audioSource.PlayOneShot(monsterAttack);
            }

            if (deathScream != null)
            {
                audioSource.PlayOneShot(deathScream);
            }
        }

        // Disable player control
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Quick fade to black
        yield return StartCoroutine(FadeToBlack());

        // Brief pause
        yield return new WaitForSeconds(1f);

        // Show game over screen
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (gameOverText != null)
            {
                gameOverText.text = deathMessage;
            }
        }

        // Wait for input
        yield return new WaitForSeconds(2f);

        float timer = 0f;
        while (timer < 5f)
        {
            if (Input.anyKeyDown)
            {
                LoadMenu();
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Auto-load menu after 5 seconds
        LoadMenu();
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1;
        fadePanel.color = color;
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void OnDrawGizmosSelected()
    {
        // Visualize death range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deathRange);
    }
}