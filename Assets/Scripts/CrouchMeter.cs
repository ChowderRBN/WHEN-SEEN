using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouchMeter : MonoBehaviour
{
    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 5f;

    [Header("References")]
    public CharacterController controller;
    public NoiseDetection noiseDetection;

    private bool isCrouching = false;
    private InputActionAsset inputActions;


    void Update()
    {
        UpdateCrouchAnimation();
    }

    void StartCrouch()
    {
        isCrouching = true;
        if (noiseDetection != null)
        {
            noiseDetection.SetCrouching(true);
        }
    }

    void StopCrouch()
    {
        isCrouching = false;
        if (noiseDetection != null)
        {
            noiseDetection.SetCrouching(false);
        }
    }

    void UpdateCrouchAnimation()
    {
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSpeed);
    }

    public void OnCrouch(InputValue value)
    {
        bool crouchPressed = value.isPressed;
        if (crouchPressed && !isCrouching)
        {
            StartCrouch();
        }
        else if (!crouchPressed && isCrouching)
        {
            StopCrouch();
        }
    }

}

