using UnityEngine;
using UnityEngine.InputSystem;

public class TPSCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float targetHeight = 1.6f;   // look point on the player

    [Header("Aim")]
    public float mouseXSensitivity = 200f;  // player yaw
    public float mouseYSensitivity = 200f;  // camera pitch
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Zoom")]
    public float distance = 3.5f;
    public float minDistance = 2.0f;
    public float maxDistance = 6.0f;
    public float zoomSpeed = 6f;

    [Header("Smoothing")]
    public float followSharpness = 20f;    // position smoothing

    [Header("Collision")]
    public LayerMask collisionMask = ~0;
    public float collisionRadius = 0.2f;

    [Header("Input System")]
    public InputActionReference lookAction;   // <Mouse>/delta or <Pointer>/delta
    public InputActionReference zoomAction;   // <Mouse>/scroll/y

    [Header("UX")]
    public bool lockCursor = true;

    float pitch = 15f; // only camera pitch; yaw comes from rotating the player

    void OnEnable()
    {
        if (lookAction) lookAction.action.Enable();
        if (zoomAction) zoomAction.action.Enable();
        if (lockCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    }

    void OnDisable()
    {
        if (lookAction) lookAction.action.Disable();
        if (zoomAction) zoomAction.action.Disable();
        Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (!target) return;

        // --- Input ---
        Vector2 look = lookAction ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
        float scroll = zoomAction ? zoomAction.action.ReadValue<float>() : 0f;

        // Rotate PLAYER around Y with mouse X (TPS style)
        if (look.x != 0f)
            target.Rotate(0f, look.x * mouseXSensitivity * Time.deltaTime, 0f, Space.Self);

        // Pitch CAMERA with mouse Y
        pitch -= look.y * mouseYSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Zoom (clamped)
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
        {
            float scaled = scroll * zoomSpeed * Time.deltaTime;
            if (Mathf.Abs(scroll) > 10f) scaled *= 0.02f; // tame large deltas
            distance = Mathf.Clamp(distance - scaled, minDistance, maxDistance);
        }

        // Build transform (yaw from player, pitch from camera)
        Quaternion rot = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        Vector3 focus = target.position + Vector3.up * targetHeight;

        // Collision pull-in
        float finalDist = distance;
        Vector3 toCam = rot * new Vector3(0, 0, -finalDist);
        Vector3 desiredPos = focus + toCam;

        if (Physics.SphereCast(focus, collisionRadius, toCam.normalized, out RaycastHit hit, finalDist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            finalDist = Mathf.Clamp(hit.distance - 0.05f, minDistance, finalDist);
            desiredPos = focus + rot * new Vector3(0, 0, -finalDist);
        }

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
        transform.rotation = rot;
        transform.LookAt(focus);
    }
}
