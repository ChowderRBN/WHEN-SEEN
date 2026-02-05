using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour
{
    [Header("Scene to Load")]
    public string gameSceneName = "Game2";

    [Header("Optional")]
    public bool allowEscapeKey = true;

    void Update()
    {
        // Allow ESC key to skip
        if (allowEscapeKey && Input.GetKeyDown(KeyCode.Escape))
        {
            Skip();
        }
    }

    // Call this from the Skip Button
    public void Skip()
    {
        Debug.Log("Skipping cutscene, loading " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }
}