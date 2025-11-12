using UnityEngine;

public class DeathEventRelay : MonoBehaviour
{
    [SerializeField] private EnemyHealth target;

    void Awake()
    {
        if (!target)
            target = GetComponentInParent<EnemyHealth>();
    }


    public void OnDeathAnimationComplete()
    {
        if (target)
            target.OnDeathAnimationComplete();
        else
            Debug.LogWarning("DeathEventRelay: No EnemyHealth target found to relay event to.");
    }
}
