using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public CharacterController controller;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Look")]
    public float lookSensitivity = 2f;
    public float maxLookAngle = 90f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float currentSpeed;
    private float speedMultiplier = 1f;
    private float verticalRotation = 0f;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    [Header("Input")]
    public InputActionAsset inputActionsAsset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;

        // Load sensitivity from settings
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            lookSensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }

        // Setup input actions
        if (inputActionsAsset != null)
        {
            var playerMap = inputActionsAsset.FindActionMap("Player");
            moveAction = playerMap.FindAction("Move");
            lookAction = playerMap.FindAction("Look");
            jumpAction = playerMap.FindAction("Jump");
            sprintAction = playerMap.FindAction("Sprint");
        }
    }

    void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (lookAction != null) lookAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
        if (sprintAction != null) sprintAction.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (lookAction != null) lookAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (sprintAction != null) sprintAction.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        // Get input
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();

        bool sprintPressed = sprintAction != null && sprintAction.IsPressed();
        bool jumpPressed = jumpAction != null && jumpAction.triggered;

        // Calculate speed
        float targetSpeed = sprintPressed ? sprintSpeed : walkSpeed;
        currentSpeed = targetSpeed * speedMultiplier;

        // Move
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (jumpPressed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleLook()
    {
        if (lookAction != null)
            lookInput = lookAction.ReadValue<Vector2>();

        // Horizontal rotation
        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        // Vertical rotation
        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    // Called from PauseManager when sensitivity changes
    public void UpdateSensitivity()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            lookSensitivity = PlayerPrefs.GetFloat("Sensitivity");
            Debug.Log($"Sensitivity updated to: {lookSensitivity}");
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public void ResetSpeed()
    {
        speedMultiplier = 1f;
    }
}