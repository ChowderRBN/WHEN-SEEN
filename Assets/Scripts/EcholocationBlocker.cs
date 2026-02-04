using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EcholocationBlocker : MonoBehaviour
{
    [Header("Blocking Settings")]
    public GameObject blockingPanel; // A full-screen invisible panel
    public bool startBlocked = true;

    [Header("Echolocation Settings")]
    public KeyCode echolocationKey = KeyCode.E;
    public float unblockDelay = 3f;

    private bool isBlocked = true;
    private bool isWaiting = false;

    void Start()
    {
        if (startBlocked && blockingPanel != null)
        {
            blockingPanel.SetActive(true);
            isBlocked = true;
        }
    }

    void Update()
    {
        // Check if E is pressed and we're currently blocked and not already waiting
        if (isBlocked && !isWaiting && Input.GetKeyDown(echolocationKey))
        {
            StartCoroutine(UnblockAfterDelay());
        }
    }

    private IEnumerator UnblockAfterDelay()
    {
        isWaiting = true;

        Debug.Log("Echolocation activated! Unblocking in " + unblockDelay + " seconds...");

        // Wait for the specified delay
        yield return new WaitForSeconds(unblockDelay);

        // Unblock clicks
        UnblockClicks();

        Debug.Log("Clicks unblocked!");

        isWaiting = false;
    }

    // Call this method manually if you want to unblock immediately
    public void UnblockClicks()
    {
        if (blockingPanel != null)
        {
            blockingPanel.SetActive(false);
            isBlocked = false;
        }
    }

    // Call this to re-block clicks
    public void BlockClicks()
    {
        if (blockingPanel != null)
        {
            blockingPanel.SetActive(true);
            isBlocked = true;
        }
    }

    // Call this to reset the blocker (useful for restarting level or game)
    public void ResetBlocker()
    {
        StopAllCoroutines();
        isWaiting = false;
        BlockClicks();
    }
}