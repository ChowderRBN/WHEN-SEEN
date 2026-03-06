using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour
{
    public void LoadGame2()
    {
        SceneManager.LoadScene("Game2");
    }
}