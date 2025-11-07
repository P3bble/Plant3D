using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMover : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    public InputActionReference aimAction; // optional, jump still works while aiming

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float airControl = 0.5f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.1f;      // small grace after leaving ground
    public float jumpBuffer = 0.1f;      // small grace before touching ground

    [Header("Animation")]
    public Animator animator;
    public string moveBool = "IsMoving";
    public string groundedBool = "IsGrounded";
    public string jumpTrigger = "Jump";

    CharacterController controller;
    float yVel;
    float coyoteTimer;
    float jumpBufferTimer;
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
        if (aimAction) aimAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction) moveAction.action.Disable();
        if (jumpAction) jumpAction.action.Disable();
        if (sprintAction) sprintAction.action.Disable();
        if (aimAction) aimAction.action.Disable();
    }

    void Update()
    {
        Vector2 input = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        bool sprinting = sprintAction && sprintAction.action.IsPressed();
        bool aiming = aimAction && aimAction.action.IsPressed();
        float targetSpeed = sprinting ? sprintSpeed : walkSpeed;

        // move on the plane
        Vector3 moveDir = transform.forward * input.y + transform.right * input.x;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontal = controller.isGrounded
            ? moveDir * targetSpeed
            : moveDir * targetSpeed * Mathf.Clamp01(airControl);

        // track grounded with a small grace window
        bool grounded = controller.isGrounded;
        if (grounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        // buffer jump input
        if (jumpAction && jumpAction.action.WasPressedThisFrame())
            jumpBufferTimer = jumpBuffer;
        else
            jumpBufferTimer -= Time.deltaTime;

        // do the jump (works while sprinting or aiming)
        if (coyoteTimer > 0f && jumpBufferTimer > 0f)
        {
            yVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            if (animator) animator.SetTrigger(jumpTrigger);
        }

        // stick to ground a bit
        if (grounded && yVel < 0f) yVel = -2f;

        // gravity
        yVel += gravity * Time.deltaTime;

        Vector3 velocity = new Vector3(horizontal.x, yVel, horizontal.z);
        controller.Move(velocity * Time.deltaTime);

        // anims
        if (animator)
        {
            Vector3 hv = controller.velocity; hv.y = 0f;
            animator.SetBool(moveBool, hv.sqrMagnitude > 0.01f);
            animator.SetBool(groundedBool, grounded);
        }

        wasGrounded = grounded;
    }
}
