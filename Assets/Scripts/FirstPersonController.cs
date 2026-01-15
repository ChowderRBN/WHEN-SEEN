using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 5f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;       // normal speed
    public float runSpeed = 5f;        // normal run speed
    private float currentSpeed;

    private CharacterController controller;
    private bool isCrouching = false;

    // **PUBLIC PROPERTIES**
    public bool IsCrouching => isCrouching; // read-only access for other scripts
    public float CurrentSpeed => currentSpeed; // current movement speed

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkSpeed;
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

        // Move camera smoothly with crouch
        Vector3 camPos = playerCamera.localPosition;
        float targetCamY = isCrouching ? crouchHeight - 0.1f : standingHeight - 0.4f;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * crouchTransitionSpeed);
        playerCamera.localPosition = camPos;
    }

    void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= currentSpeed * Time.deltaTime;

        controller.Move(move);
    }

    // **METHOD TO SET SPEED** (for adrenaline boost)
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeed = runSpeed * multiplier;
    }

    // **METHOD TO RESET SPEED** (after adrenaline ends)
    public void ResetSpeed()
    {
        currentSpeed = walkSpeed;
    }

    // **NEW METHOD: Force player to stand**
    public void ForceStand()
    {
        isCrouching = false;
    }
}
