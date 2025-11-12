using UnityEngine;
using UnityEngine.InputSystem;

public class TPSCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float targetHeight = 1.6f;

    [Header("Look")]
    public float mouseXSensitivity = 200f;
    public float mouseYSensitivity = 200f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Distance")]
    public float distance = 6f;
    public float minDistance = 1.0f;
    public float maxDistance = 15.0f;

    [Header("Aim View")]
    public float aimDistance = 2.0f;  // zoom
    public float shoulderX = 0.0f;
    public float aimShoulderX = 0.5f;
    public float shoulderY = 0.2f;
    public float offsetBlend = 12f;

    [Header("Smoothing")]
    public float followSharpness = 20f;

    [Header("Collision")]
    public LayerMask collisionMask = ~0;
    public float collisionRadius = 0.2f;

    [Header("Input System")]
    public InputActionReference lookAction;
    public InputActionReference aimAction;

    [Header("Animation")]
    public Animator animator;
    public string aimBool = "IsAiming";

    [Header("UX")]
    public bool lockCursor = true;

    float pitch = 15f;
    float targetDistance;
    float sideOffset;

    void OnEnable()
    {
        if (lookAction) lookAction.action.Enable();
        if (aimAction) aimAction.action.Enable();
        if (lockCursor && !(SettingsUI.IsOpen))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

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
        if (SettingsUI.IsOpen) return;

        bool isAiming = aimAction && aimAction.action.IsPressed();
        if (animator) animator.SetBool(aimBool, isAiming);

        Vector2 look = lookAction ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;

        float sens = SettingsUI.MouseSensitivity;
        if (look.x != 0f)
            target.Rotate(0f, look.x * mouseXSensitivity * sens * Time.deltaTime, 0f, Space.Self);

        pitch -= look.y * mouseYSensitivity * sens * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);


        float desiredDist = Mathf.Clamp(isAiming ? aimDistance : distance, minDistance, maxDistance);
        targetDistance = desiredDist;

        float desiredSide = (isAiming ? shoulderX + aimShoulderX : shoulderX);
        sideOffset = Mathf.Lerp(sideOffset, desiredSide, 1f - Mathf.Exp(-offsetBlend * Time.deltaTime));

        Quaternion rot = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        Vector3 focus = target.position + Vector3.up * targetHeight;

        Vector3 side = rot * (Vector3.right * sideOffset + Vector3.up * shoulderY);
        Vector3 back = rot * new Vector3(0f, 0f, -targetDistance);
        Vector3 desiredPos = focus + side + back;

        Vector3 dir = desiredPos - focus;
        float dist = dir.magnitude;
        if (dist > 0.0001f)
        {
            dir /= dist;
            if (Physics.SphereCast(focus, collisionRadius, dir, out RaycastHit hit, dist, collisionMask, QueryTriggerInteraction.Ignore))
                desiredPos = focus + dir * Mathf.Clamp(hit.distance - 0.05f, minDistance, dist);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
        transform.rotation = rot;
        transform.LookAt(focus);
    }
}
