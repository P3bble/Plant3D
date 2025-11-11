using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class EnemyHPBar : MonoBehaviour
{
    [SerializeField] CanvasGroup hpCanvasGroup;
    [SerializeField] Transform targetEnemy; // assign when enemy spawns
    Camera mainCam;

    void Awake()
    {
        if (!hpCanvasGroup) hpCanvasGroup = GetComponent<CanvasGroup>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (mainCam)
            transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                             mainCam.transform.rotation * Vector3.up);

        if (targetEnemy)
            transform.position = targetEnemy.position + Vector3.up * 2f; // tweak offset
    }

    public void SetVisible(bool visible)
    {
        if (hpCanvasGroup) hpCanvasGroup.alpha = visible ? 1f : 0.4f;
    }

    public void AttachToEnemy(Transform enemy)
    {
        targetEnemy = enemy;
    }
}
