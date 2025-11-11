using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;

    void Awake() => cam = Camera.main;

    void LateUpdate()
    {
        if (!cam) { cam = Camera.main; if (!cam) return; }
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
                         cam.transform.rotation * Vector3.up);
    }
}
