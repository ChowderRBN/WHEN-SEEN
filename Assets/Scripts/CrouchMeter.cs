using UnityEngine;

public class PlayerCrouchMeter : MonoBehaviour
{
    [Header("Crouch Settings")]
    public FirstPersonController controller;
    public float maxTime = 10f;
    public float recoveryRate = 2f;

    [Header("Noise Detection")]
    public NoiseDetection noiseDetection;

    private float meter;

    void Start()
    {
        meter = maxTime;
    }

    void Update()
    {
        if (controller.IsCrouching)
        {
            meter -= Time.deltaTime;

            // Notify noise system that player is crouching
            if (noiseDetection != null)
            {
                noiseDetection.SetCrouching(true);
            }

            if (meter <= 0f)
            {
                controller.ForceStand();
            }
        }
        else
        {
            meter = Mathf.Min(meter + recoveryRate * Time.deltaTime, maxTime);

            // Notify noise system that player is not crouching
            if (noiseDetection != null)
            {
                noiseDetection.SetCrouching(false);
            }
        }
    }
}