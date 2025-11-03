using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMover : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float airControl = 0.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Animation")]
    public Animator animator;
    public string moveBool = "IsMoving";   
    public string groundedBool = "IsGrounded";
    public string jumpTrigger = "Jump";    // 


    CharacterController controller;
    Vector3 verticalVelocity;
    bool wasGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }

    void OnEnable()
    {
        if (moveAction) moveAction.action.Enable();
        if (jumpAction) jumpAction.action.Enable();
        if (sprintAction) sprintAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction) moveAction.action.Disable();
        if (jumpAction) jumpAction.action.Disable();
        if (sprintAction) sprintAction.action.Disable();
    }

    void Update()
    {
        Vector2 input = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        bool sprinting = sprintAction && sprintAction.action.IsPressed();
        float targetSpeed = sprinting ? sprintSpeed : walkSpeed;

        Vector3 moveDir = transform.forward * input.y + transform.right * input.x;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontal = controller.isGrounded
            ? moveDir * targetSpeed
            : moveDir * targetSpeed * Mathf.Clamp01(airControl);

        // jump
        if (controller.isGrounded)
        {
            if (verticalVelocity.y < 0f) verticalVelocity.y = -2f;
            if (jumpAction && jumpAction.action.WasPressedThisFrame())
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (animator) animator.SetTrigger(jumpTrigger);
            }
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 velocity = new Vector3(horizontal.x, verticalVelocity.y, horizontal.z);
        controller.Move(velocity * Time.deltaTime);

        // animation parameters
        if (animator)
        {
            Vector3 hv = controller.velocity; hv.y = 0f;
            animator.SetBool(moveBool, hv.sqrMagnitude > 0.01f);

            bool grounded = controller.isGrounded;
            animator.SetBool(groundedBool, grounded);

         // land trigger

            wasGrounded = grounded;
        }
    }
}
