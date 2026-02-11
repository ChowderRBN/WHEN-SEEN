using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SlopeController : MonoBehaviour
{
    [Header("Slope Settings")]
    public float maxSlopeAngle = 45f; // Maximum walkable slope angle
    public float slideSpeed = 8f; // How fast you slide down steep slopes
    public bool preventSlopeClimbing = true; // Prevent climbing steep slopes

    private CharacterController controller;
    private Vector3 slideVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckSlope();
    }

    void CheckSlope()
    {
        if (controller.isGrounded)
        {
            RaycastHit hit;

            // Cast ray down to detect slope
            if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 + 0.3f))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

                if (slopeAngle > maxSlopeAngle)
                {
                    // On too-steep slope
                    if (preventSlopeClimbing)
                    {
                        // Slide down the slope
                        Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal);
                        slideVelocity = slideDirection * slideSpeed;
                        controller.Move(slideVelocity * Time.deltaTime);
                    }
                }
                else
                {
                    slideVelocity = Vector3.zero;
                }
            }
        }
    }
}