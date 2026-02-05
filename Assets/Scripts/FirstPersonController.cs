using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 5f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float baseSensitivity = 100f; // Base sensitivity (you can adjust this)
    private float mouseSensitivity; // Actual sensitivity used (loaded from settings)
    private float xRotation = 0f;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    private float currentSpeed;

    private CharacterController controller;
    private bool isCrouching = false;

    public bool IsCrouching => isCrouching;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;

        // Load sensitivity from PlayerPrefs
        LoadSensitivity();
    }

    void Update()
    {
        LookAround();
        HandleCrouch();
        HandleMovement();
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = !isCrouching;

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        Vector3 camPos = playerCamera.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, isCrouching ? crouchHeight - 0.1f : standingHeight - 0.4f,
                              Time.deltaTime * crouchTransitionSpeed);
        playerCamera.localPosition = camPos;
    }

    void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    // Load sensitivity from PlayerPrefs
    void LoadSensitivity()
    {
        // Get saved sensitivity (default 2.0 if not set)
        float savedSensitivity = PlayerPrefs.GetFloat("Sensitivity", 2.0f);

        // Apply to mouse sensitivity
        mouseSensitivity = baseSensitivity * savedSensitivity;

        Debug.Log("Mouse sensitivity loaded: " + mouseSensitivity + " (Base: " + baseSensitivity + " x Multiplier: " + savedSensitivity + ")");
    }

    // Call this to update sensitivity without restarting
    public void UpdateSensitivity()
    {
        LoadSensitivity();
    }

    public void SetSpeedMultiplier(float multiplier) => currentSpeed = runSpeed * multiplier;
    public void ResetSpeed() => currentSpeed = walkSpeed;
    public void ForceStand() => isCrouching = false;
}