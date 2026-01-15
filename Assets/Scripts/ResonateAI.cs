using UnityEngine;

public class ResonateAI : MonoBehaviour
{
    public void Alert(Vector3 playerPosition, float loudness)
    {
        // This is called when the player makes noise
        // TODO: Add AI logic to move towards player
        Debug.Log($"{name} alerted by noise at {playerPosition} with loudness {loudness}");
    }

    public void Flee(Vector3 sourcePosition)
    {
        // Called when player uses sonic scream
        // TODO: Add flee AI logic
        Debug.Log($"{name} flees from scream at {sourcePosition}");
    }
}
