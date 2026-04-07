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

    [Header("Crouch")]
    public float crouchSpeed = 2.5f;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 10f;

    [Header("Look")]
    public float lookSensitivity = 2f;
    public float maxLookAngle = 90f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float currentSpeed;
    private float speedMultiplier = 1f;
    private float verticalRotation = 0f;
    private bool isCrouching = false;

    // --- NEW: state bools driven by both hardware input and virtual buttons ---
    private bool sprintState = false;
    private bool crouchState = false;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    [Header("Input")]
    public InputActionAsset inputActionsAsset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;
        standingHeight = controller.height;

        if (PlayerPrefs.HasKey("Sensitivity"))
            lookSensitivity = PlayerPrefs.GetFloat("Sensitivity");

        if (inputActionsAsset != null)
        {
            var playerMap = inputActionsAsset.FindActionMap("Player");
            moveAction = playerMap.FindAction("Move");
            lookAction = playerMap.FindAction("Look");
            jumpAction = playerMap.FindAction("Jump");
            sprintAction = playerMap.FindAction("Sprint");
            crouchAction = playerMap.FindAction("Crouch");
        }
    }

    void OnEnable()
    {
        moveAction?.Enable();
        lookAction?.Enable();
        jumpAction?.Enable();
        sprintAction?.Enable();
        crouchAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        lookAction?.Disable();
        jumpAction?.Disable();
        sprintAction?.Disable();
        crouchAction?.Disable();
    }

    // -------------------------------------------------------------------------
    // Public input methods — mirrors ScreamInput pattern
    // -------------------------------------------------------------------------
    public void MoveInput(Vector2 input) => moveInput = input.normalized;
    public void LookInput(Vector2 input) => lookInput = input.normalized;

    /// <summary>Called by UICanvasControllerInput.VirtualSprintInput or a UI button.</summary>
    public void SprintInput(bool sprintPressed) => sprintState = sprintPressed;

    /// <summary>Called by UICanvasControllerInput.VirtualCrouchInput or a UI button.</summary>
    public void CrouchInput(bool crouchPressed) => crouchState = crouchPressed;

    // -------------------------------------------------------------------------

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
#if !UNITY_IOS && !UNITY_ANDROID
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();

        // Merge hardware input INTO the shared state bools so both paths work
        if (sprintAction != null)
            sprintState = sprintAction.IsPressed();

        if (crouchAction != null)
            crouchState = crouchAction.IsPressed();
#endif

        bool jumpPressed = jumpAction != null && jumpAction.triggered;

        // Sprint cancels crouch; crouch cancels sprint
        isCrouching = crouchState && !sprintState;

        // Smoothly adjust controller height
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight,
                                       Time.deltaTime * crouchTransitionSpeed);

        // Speed priority: crouching > sprinting > walking
        float targetSpeed = isCrouching ? crouchSpeed
                          : sprintState ? sprintSpeed
                          : walkSpeed;
        currentSpeed = targetSpeed * speedMultiplier;

        // Lateral / forward movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump (disabled while crouching)
        if (jumpPressed && controller.isGrounded && !isCrouching)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
#if !UNITY_IOS && !UNITY_ANDROID
        if (lookAction != null)
            lookInput = lookAction.ReadValue<Vector2>();
#endif

        transform.Rotate(Vector3.up * lookInput.x * lookSensitivity);

        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    public void UpdateSensitivity()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            lookSensitivity = PlayerPrefs.GetFloat("Sensitivity");
            Debug.Log($"Sensitivity updated to: {lookSensitivity}");
        }
    }

    public void SetSpeedMultiplier(float multiplier) => speedMultiplier = multiplier;
    public void ResetSpeed() => speedMultiplier = 1f;
}