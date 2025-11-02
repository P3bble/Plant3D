using UnityEngine;
using UnityEngine.InputSystem;

public class TPSCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float targetHeight = 1.6f;

    [Header("Aim")]
    public float mouseXSensitivity = 200f;
    public float mouseYSensitivity = 200f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Distance")]
    public float distance = 6f;
    public float minDistance = 1.0f;
    public float maxDistance = 15.0f;

    [Header("Aim Zoom")]
    public float aimDistance = 2.0f;

    [Header("Smoothing")]
    public float followSharpness = 20f;

    [Header("Collision")]
    public LayerMask collisionMask = ~0;
    public float collisionRadius = 0.2f;

    [Header("Input System")]
    public InputActionReference lookAction;
    public InputActionReference aimAction;

    [Header("Animation")]
    public Animator animator;          // drag Isaac's Animator here
    public string aimBool = "IsAiming";

    [Header("UX")]
    public bool lockCursor = true;

    float pitch = 15f;
    float targetDistance;

    void OnEnable()
    {
        if (lookAction) lookAction.action.Enable();
        if (aimAction) aimAction.action.Enable();

        if (lockCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        targetDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        if (!animator) animator = GetComponentInChildren<Animator>(true);
    }

    void OnDisable()
    {
        if (lookAction) lookAction.action.Disable();
        if (aimAction) aimAction.action.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (!target) return;

        bool isAiming = aimAction && aimAction.action.IsPressed();
        if (animator) animator.SetBool(aimBool, isAiming);

        Vector2 look = lookAction ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;

        if (look.x != 0f)
            target.Rotate(0f, look.x * mouseXSensitivity * Time.deltaTime, 0f, Space.Self);

        pitch -= look.y * mouseYSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        float desiredZoom = Mathf.Clamp(isAiming ? aimDistance : distance, minDistance, maxDistance);
        targetDistance = desiredZoom;

        Quaternion rot = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        Vector3 focus = target.position + Vector3.up * targetHeight;

        float finalDist = targetDistance;
        Vector3 toCam = rot * new Vector3(0, 0, -finalDist);
        Vector3 desiredPos = focus + toCam;

        if (Physics.SphereCast(focus, collisionRadius, toCam.normalized, out RaycastHit hit, finalDist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            finalDist = Mathf.Clamp(hit.distance - 0.05f, minDistance, finalDist);
            desiredPos = focus + rot * new Vector3(0, 0, -finalDist);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
        transform.rotation = rot;
        transform.LookAt(focus);
    }
}
