using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEcholocation : MonoBehaviour
{
    [Header("References")]
    public FirstPersonController fpController;

    [Header("Echolocation")]
    public GameObject echoWavePrefab;       // Big manual wave prefab
    public GameObject miniStepWavePrefab;   // Small step wave prefab

    [Header("Ground Detection")]
    public LayerMask groundMask;
    public float groundCheckDistance = 3f;
    public float forwardOffset = 0.6f;

    [Header("Manual Wave Settings")]
    public int maxPingCharges = 3;
    public float pingRechargeTime = 25f;
    public float manualWaveRadius = 12f;
    public float manualWaveDuration = 1f;      // How long manual wave stays
    public float manualWaveSpeed = 6f;         // How fast the wave expands

    [Header("Step Wave Settings")]
    public float stepWaveDistance = 0.25f;     // Distance per step
    public float stepWaveRadius = 1.2f;        // Step wave max radius
    public float stepWaveDuration = 0.5f;      // How long the step wave stays
    public float stepWaveSpeed = 4f;           // How fast step wave expands

    private int currentPingCharges;
    private bool canPing = true;
    private Vector3 lastStepPosition;

    [Header("Crouch Meter")]
    public float maxCrouchTime = 10f;
    public float crouchRecoveryRate = 2f;
    private float crouchMeter;

    [Header("Heartbeat & Adrenaline")]
    public float heartbeatThreshold = 5f;
    public float adrenalineDuration = 10f;
    private bool adrenalineActive = false;

    [Header("Sonic Screams")]
    public int maxScreams = 2;
    public float screamCooldown = 60f;
    private int currentScreams;

    [Header("Monsters")]
    public List<ResonateAI> resonates;
    public List<EchoformAI> echoforms;

    void Start()
    {
        if (fpController == null)
            fpController = GetComponent<FirstPersonController>();

        currentPingCharges = maxPingCharges;
        currentScreams = maxScreams;
        crouchMeter = maxCrouchTime;
        lastStepPosition = transform.position;
    }

    void Update()
    {
        HandleManualPing();
        HandleStepEcho();
        HandleSonicScream();
        HandleAdrenaline();
        HandleCrouchMeter();
    }

    // =========================
    // MANUAL ECHOLOCATION (E key)
    // =========================
    void HandleManualPing()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentPingCharges > 0 && canPing)
        {
            StartCoroutine(SpawnEchoWave(manualWaveRadius, echoWavePrefab, manualWaveDuration, manualWaveSpeed));
            currentPingCharges--;
            StartCoroutine(RechargePing());
        }
    }

    // =========================
    // STEP ECHO (small waves while walking)
    // =========================
    void HandleStepEcho()
    {
        if (fpController.IsCrouching)
            return; // no step echoes while crouching

        float moved = Vector3.Distance(transform.position, lastStepPosition);
        if (moved >= stepWaveDistance)
        {
            StartCoroutine(SpawnEchoWave(stepWaveRadius, miniStepWavePrefab, stepWaveDuration, stepWaveSpeed));
            lastStepPosition = transform.position;
        }
    }

    // =========================
    // SPAWN ECHO WAVE
    // =========================
    IEnumerator SpawnEchoWave(float maxRadius, GameObject wavePrefab, float duration, float speed)
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask))
            yield break;

        Vector3 origin = hit.point + transform.forward * forwardOffset + Vector3.up * 0.05f;

        GameObject wave = Instantiate(wavePrefab, origin, Quaternion.identity);
        wave.transform.localScale = Vector3.zero;

        float radius = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            radius += speed * Time.deltaTime;
            wave.transform.localScale = new Vector3(Mathf.Min(radius, maxRadius), 0.05f, Mathf.Min(radius, maxRadius));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(wave);

        // Alert monsters
        foreach (ResonateAI r in resonates)
            r.Alert(origin, maxRadius);

        foreach (EchoformAI e in echoforms)
            e.Reveal();
    }

    IEnumerator RechargePing()
    {
        canPing = false;
        yield return new WaitForSeconds(pingRechargeTime);
        currentPingCharges = Mathf.Min(currentPingCharges + 1, maxPingCharges);
        canPing = true;
    }

    // =========================
    // SONIC SCREAM
    // =========================
    void HandleSonicScream()
    {
        if (Input.GetKeyDown(KeyCode.Q) && currentScreams > 0)
        {
            foreach (ResonateAI r in resonates)
                r.Flee(transform.position);

            currentScreams--;
            StartCoroutine(RechargeScream());
        }
    }

    IEnumerator RechargeScream()
    {
        yield return new WaitForSeconds(screamCooldown);
        currentScreams = Mathf.Min(currentScreams + 1, maxScreams);
    }

    // =========================
    // HEARTBEAT & ADRENALINE
    // =========================
    void HandleAdrenaline()
    {
        if (adrenalineActive) return;

        foreach (ResonateAI r in resonates)
        {
            float dist = Vector3.Distance(transform.position, r.transform.position);
            if (dist <= heartbeatThreshold)
            {
                StartCoroutine(ActivateAdrenaline());
                break;
            }
        }
    }

    IEnumerator ActivateAdrenaline()
    {
        adrenalineActive = true;
        StartCoroutine(SpawnEchoWave(manualWaveRadius + 3f, echoWavePrefab, manualWaveDuration, manualWaveSpeed)); // bigger LONG wave
        fpController.SetSpeedMultiplier(1.65f);
        yield return new WaitForSeconds(adrenalineDuration);
        fpController.ResetSpeed();
        adrenalineActive = false;
    }

    // =========================
    // CROUCH METER
    // =========================
    void HandleCrouchMeter()
    {
        if (fpController.IsCrouching)
        {
            crouchMeter -= Time.deltaTime;
            if (crouchMeter <= 0f)
                fpController.ForceStand();
        }
        else
        {
            crouchMeter = Mathf.Min(crouchMeter + crouchRecoveryRate * Time.deltaTime, maxCrouchTime);
        }
    }
}
