using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    public Camera playerCamera;
    public Image crosshairImage;
    public float rayDistance = 100f;

    // Only these layers will be raycasted (set in Inspector).
    public LayerMask hittableLayers;     // e.g. Enemy layer (and anything else you want)

    [Range(0f, 1f)] public float idleOpacity = 0.4f;
    public float activeOpacity = 1f;

    Color crosshairColor;

    void Start()
    {
        if (crosshairImage != null)
            crosshairColor = crosshairImage.color;
    }

    void Update()
    {
        if (!playerCamera || !crosshairImage) return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        RaycastHit hit;

        // Use the LayerMask to filter what the ray can hit
        if (Physics.Raycast(ray, out hit, rayDistance, hittableLayers, QueryTriggerInteraction.Ignore))
        {
            // Optional: Debug who we hit
            // Debug.Log("Hit: " + hit.collider.name);

            // Check tag if you still want the tag gate
            if (hit.collider.CompareTag("Enemy"))
                SetCrosshairAlpha(activeOpacity);
            else
                SetCrosshairAlpha(idleOpacity);
        }
        else
        {
            SetCrosshairAlpha(idleOpacity);
        }

        // Visualize the ray in Scene view (white = miss, green = hit)
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, crosshairImage.color.a >= activeOpacity ? Color.green : Color.white);
    }

    void SetCrosshairAlpha(float a)
    {
        crosshairColor.a = a;
        crosshairImage.color = crosshairColor;
    }

    // Handy auto-wiring in editor
    void OnValidate()
    {
        if (!playerCamera) playerCamera = Camera.main;
        if (!crosshairImage) crosshairImage = GetComponent<Image>();
    }
}
