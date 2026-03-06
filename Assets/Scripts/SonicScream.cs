using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SonicScream : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;
    public AudioClip clip;

    [Header("Scream Settings")]
    public int maxScreams = 2;
    public float cooldown = 60f;
    public float fleeDistance = 50f;
    public float spawnBlockDuration = 5f;

    [Header("Noise Detection")]
    public NoiseDetection noiseDetection;

    [Header("Spawner")]
    public NoiseMonsterSpawner spawner;

    [Header("Adrenaline")]
    public PlayerAdrenaline adrenaline;

    [Header("Input")]
    public InputActionAsset inputActionsAsset;

    private int screams;
    private InputAction sonicScreamAction;

    void Start()
    {
        screams = maxScreams;

        if (noiseDetection == null)
            noiseDetection = GetComponent<NoiseDetection>();

        if (adrenaline == null)
            adrenaline = GetComponent<PlayerAdrenaline>();

        if (spawner == null)
            spawner = FindObjectOfType<NoiseMonsterSpawner>();

        if (inputActionsAsset != null)
        {
            sonicScreamAction = inputActionsAsset.FindActionMap("Player").FindAction("SonicScream");
        }
    }

    void OnEnable()
    {
        if (sonicScreamAction != null)
        {
            sonicScreamAction.Enable();
            sonicScreamAction.performed += HandleSonicScream;
        }
    }

    void OnDisable()
    {
        if (sonicScreamAction != null)
        {
            sonicScreamAction.performed -= HandleSonicScream;
            sonicScreamAction.Disable();
        }
    }

    void HandleSonicScream(InputAction.CallbackContext context)
    {
        if (screams > 0)
        {
            UseSonicScream();
        }
    }

    void UseSonicScream()
    {
        Debug.Log("SONIC SCREAM USED!");

        if (source != null && clip != null)
        {
            source.PlayOneShot(clip);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int enemiesAffected = 0;

        foreach (GameObject enemyObj in enemies)
        {
            ResonateAI resonate = enemyObj.GetComponent<ResonateAI>();
            if (resonate != null)
            {
                resonate.Flee(transform.position, fleeDistance);
                enemiesAffected++;
            }
        }

        Debug.Log($"Sonic Scream affected {enemiesAffected} enemies");

        if (adrenaline != null)
            adrenaline.TriggerAdrenaline();

        if (noiseDetection != null)
            noiseDetection.NotifySonicScreamUsed();

        if (spawner != null)
            spawner.BlockSpawning(spawnBlockDuration);

        screams--;
        StartCoroutine(Recharge());
    }

    IEnumerator Recharge()
    {
        yield return new WaitForSeconds(cooldown);
        screams = Mathf.Min(screams + 1, maxScreams);
        Debug.Log($"Sonic Scream recharged! ({screams}/{maxScreams})");
    }

    public int GetCurrentScreams() => screams;
    public int GetMaxScreams() => maxScreams;
}