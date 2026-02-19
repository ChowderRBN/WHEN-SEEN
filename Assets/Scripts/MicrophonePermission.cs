using UnityEngine;
using System.Collections;

public class MicrophonePermission : MonoBehaviour
{
    public MicrophoneNoiseDetection micDetection;
    public GameObject permissionPanel;

    IEnumerator Start()
    {
        // Request microphone permission
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("Microphone permission granted!");

            if (micDetection != null)
            {
                micDetection.useMicrophone = true;
            }
        }
        else
        {
            Debug.LogWarning("Microphone permission denied!");

            if (permissionPanel != null)
            {
                permissionPanel.SetActive(true);
            }
        }
    }
}