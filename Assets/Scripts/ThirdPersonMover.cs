using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float turnSmoothTime = 0.1f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cameraTransform;

    CharacterController controller;
    float turnSmoothVelocity;
    Vector3 velocity; // for vertical motion (gravity)

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Ground check & gravity reset
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0f)
            velocity.y = -2f; // small downward force keeps us grounded

        // Input
        float x = Input.GetAxisRaw("Horizontal");   // A/D or Left/Right
        float z = Input.GetAxisRaw("Vertical");     // W/S or Up/Down
        Vector3 inputDir = new Vector3(x, 0f, z).normalized;

        // Move relative to camera
        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // Jump
        if (grounded && Input.GetButtonDown("Jump"))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
