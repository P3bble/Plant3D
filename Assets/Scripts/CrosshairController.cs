using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    public Camera playerCamera;
    public float detectRadius = 10f;
    public float detectRange = 1000f;
    public LayerMask hittableLayers;
    private EnemyHPBar currentTarget;

    void Update()
    {
        if (!playerCamera) return;

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        if (Physics.SphereCast(ray, detectRadius, out hit, detectRange, hittableLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // If HP bars aren’t children, you can store a link on the enemy to its bar instead.
                EnemyHPBar newTarget = hit.collider.GetComponentInChildren<EnemyHPBar>();
                if (newTarget != currentTarget)
                {
                    if (currentTarget) currentTarget.SetVisible(false);
                    if (newTarget) newTarget.SetVisible(true);
                    currentTarget = newTarget;
                }
            }
        }
        else
        {
            if (currentTarget)
            {
                currentTarget.SetVisible(false);
                currentTarget = null;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!playerCamera) return;
        Gizmos.color = Color.cyan;
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        Gizmos.DrawRay(ray.origin, ray.direction * 5f);
        Gizmos.DrawWireSphere(ray.origin + ray.direction * 5f, detectRadius);
    }
}
