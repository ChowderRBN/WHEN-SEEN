using UnityEngine;

public class EchoformAI : MonoBehaviour
{
    private bool isRevealed = false;

    public void Reveal()
    {
        // Called when player echolocation hits this enemy
        isRevealed = true;
        // TODO: Add visual cue / enable renderer
        Debug.Log($"{name} revealed by echolocation!");
    }

    public void Hide()
    {
        isRevealed = false;
        // TODO: Hide the enemy again
        Debug.Log($"{name} hidden");
    }
}
