using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMover : MonoBehaviour
{
    [Header("References")]
    // cameraTransform no longer needed for movement-relative-to-player,
    // but you can keep it if other systems use it.
    public Transform cameraTransform;

    public InputActionReference moveAction;
    public InputActionReference jumpAction;   // optional
    public InputActionReference sprintAction; // optional

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float airControl = 0.5f;     // 0..1, how much control you have in air
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    private CharacterController controller;
    private Vector3 verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        if (jumpAction) jumpAction.action.Enable();
        if (sprintAction) sprintAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        if (jumpAction) jumpAction.action.Disable();
        if (sprintAction) sprintAction.action.Disable();
    }

    void Update()
    {
        // --- Read input ---
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        bool isSprinting = sprintAction && sprintAction.action.IsPressed();
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // --- Move relative to PLAYER orientation (TPS strafe) ---
        // Mouse (camera script) should rotate the PLAYER (yaw).
        Vector3 moveDir = (transform.forward * input.y) + (transform.right * input.x);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // Grounded movement vs air control
        Vector3 horizontalMove;
        if (controller.isGrounded)
        {
            horizontalMove = moveDir * targetSpeed;
        }
        else
        {
            // limited control in air
            horizontalMove = moveDir * targetSpeed * Mathf.Clamp01(airControl);
        }

        // --- Jump / Gravity ---
        if (controller.isGrounded)
        {
            // small downward force keeps us grounded on slopes
            if (verticalVelocity.y < 0f) verticalVelocity.y = -2f;

            if (jumpAction && jumpAction.action.WasPressedThisFrame())
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        // --- Apply movement ---
        Vector3 velocity = new Vector3(horizontalMove.x, verticalVelocity.y, horizontalMove.z);
        controller.Move(velocity * Time.deltaTime);
    }
}
