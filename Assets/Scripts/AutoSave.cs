using System.Collections;
using UnityEngine;

public class AutoSave : MonoBehaviour
{
    [Header("Auto-Save Settings")]
    public float autoSaveInterval = 300f; // Auto-save every 5 minutes (300 seconds)
    public bool enableAutoSave = true;

    private float timeSinceLastSave = 0f;

    void Update()
    {
        if (!enableAutoSave) return;

        timeSinceLastSave += Time.deltaTime;

        if (timeSinceLastSave >= autoSaveInterval)
        {
            PerformAutoSave();
            timeSinceLastSave = 0f;
        }
    }

    public void PerformAutoSave()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentSaveSlot != -1)
        {
            // Update player position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GameManager.Instance.playerPosition = player.transform.position;
            }

            GameManager.Instance.SaveGame();
            Debug.Log("Auto-Save Complete!");

            // Optional: Show notification to player
            ShowAutoSaveNotification();
        }
    }

    void ShowAutoSaveNotification()
    {
        // You can add a UI notification here
        Debug.Log("Game Auto-Saved!");
    }

    // Call this from checkpoints or important events
    public void TriggerAutoSave()
    {
        PerformAutoSave();
    }
}