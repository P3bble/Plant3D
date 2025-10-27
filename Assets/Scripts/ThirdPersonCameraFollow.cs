using UnityEngine;

public class ThirdPersonCameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public float followSpeed = 10f;

    [Header("Mouse Orbit Settings")]
    public float mouseSensitivity = 200f;
    private float yaw;
    private float pitch;

    void LateUpdate()
    {
        if (!target) return;

        // --- Mouse orbit ---
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -30f, 70f);

        // --- Calculate camera position ---
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPos = target.position + rotation * offset;

        // --- Smooth follow ---
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // --- Look at player ---
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
