using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;   // ✅ only this one

public class CameraAimSwitcher : MonoBehaviour
{
    public CinemachineCamera thirdPersonCamera;
    public CinemachineCamera aimCamera;
    public InputActionReference aimAction;

    int basePriority;

    void Awake()
    {
        basePriority = thirdPersonCamera.Priority;
        aimCamera.Priority = basePriority - 1;
    }

    void OnEnable() => aimAction.action.Enable();
    void OnDisable() => aimAction.action.Disable();

    void Update()
    {
        bool aiming = aimAction.action.IsPressed();
        aimCamera.Priority = aiming ? basePriority + 1 : basePriority - 1;
        thirdPersonCamera.Priority = basePriority;
    }
}
