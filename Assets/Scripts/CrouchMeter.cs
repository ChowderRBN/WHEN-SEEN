using UnityEngine;

public class PlayerCrouchMeter : MonoBehaviour
{
    public FirstPersonController controller;

    public float maxTime = 10f;
    public float recoveryRate = 2f;

    float meter;

    void Start()
    {
        meter = maxTime;
    }

    void Update()
    {
        if (controller.IsCrouching)
        {
            meter -= Time.deltaTime;
            if (meter <= 0f)
                controller.ForceStand();
        }
        else
        {
            meter = Mathf.Min(meter + recoveryRate * Time.deltaTime, maxTime);
        }
    }
}
