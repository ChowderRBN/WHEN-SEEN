using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEcholocation : MonoBehaviour
{
    [Header("References")]
    public FirstPersonController fpController; // Reference to your movement/crouch script
    public LayerMask echolocationLayerMask;
    public GameObject echolocationEffectPrefab;

    [Header("Echolocation Settings")]
    public int maxPingCharges = 3;
    public float pingRechargeTime = 25f;
    private int currentPingCharges;
    private bool canPing = true;
    public float stepEchoDistance = 0.25f;
    private Vector3 lastStepPosition;

    [Header("Crouch Meter")]
    public float maxCrouchTime = 10f;
    public float crouchRecoveryRate = 2f;
    private float crouchMeter;
    private bool crouchTired = false;

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
        currentPingCharges = maxPingCharges;
        currentScreams = maxScreams;
        lastStepPosition = transform.position;
        crouchMeter = maxCrouchTime;

        if (fpController == null)
            fpController = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        HandleStepEcho();
        HandleManualPing();
        HandleSonicScream();
        HandleAdrenaline();
        HandleCrouchMeter();
    }

    #region Echolocation
    void HandleStepEcho()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastStepPosition);
        if (distanceMoved >= stepEchoDistance)
        {
            EmitEcho(0.5f); // small step echo radius
            lastStepPosition = transform.position;
        }
    }

    void HandleManualPing()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentPingCharges > 0 && canPing)
        {
            EmitEcho(5f); // large ping radius
            currentPingCharges--;
            StartCoroutine(RechargePing());
        }
    }

    void EmitEcho(float radius)
    {
        // Visual effect
        if (echolocationEffectPrefab != null)
            Instantiate(echolocationEffectPrefab, transform.position, Quaternion.identity);

        // Detect objects/enemies
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, echolocationLayerMask);
        foreach (Collider hit in hits)
        {
            hit.GetComponent<EchoformAI>()?.Reveal();
        }

        // Alert Resonate based on loudness
        foreach (ResonateAI r in resonates)
        {
            r.Alert(transform.position, radius);
        }
    }

    IEnumerator RechargePing()
    {
        canPing = false;
        yield return new WaitForSeconds(pingRechargeTime);
        currentPingCharges = Mathf.Min(currentPingCharges + 1, maxPingCharges);
        canPing = true;
    }
    #endregion

    #region Sonic Screams
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
    #endregion

    #region Adrenaline
    void HandleAdrenaline()
    {
        foreach (ResonateAI r in resonates)
        {
            float dist = Vector3.Distance(transform.position, r.transform.position);
            if (dist <= heartbeatThreshold && !adrenalineActive)
            {
                StartCoroutine(ActivateAdrenaline());
            }
        }
    }

    IEnumerator ActivateAdrenaline()
    {
        adrenalineActive = true;
        fpController.SetSpeedMultiplier(1.65f); // boost speed
        yield return new WaitForSeconds(adrenalineDuration);
        adrenalineActive = false;
        fpController.ResetSpeed(); // reset speed
    }
    #endregion

    #region Crouch Meter
    void HandleCrouchMeter()
    {
        if (fpController.IsCrouching)
        {
            crouchMeter -= Time.deltaTime;
            if (crouchMeter <= 0)
            {
                crouchTired = true;
                fpController.ForceStand(); // force player to stand safely
            }
        }
        else
        {
            if (crouchMeter < maxCrouchTime)
                crouchMeter += crouchRecoveryRate * Time.deltaTime;

            if (crouchMeter >= maxCrouchTime)
                crouchTired = false;
        }
    }
    #endregion
}
