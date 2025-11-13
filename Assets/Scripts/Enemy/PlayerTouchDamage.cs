using UnityEngine;

public class TouchDamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float attackCooldown = 1f;

    float nextAttackTime = 0f;

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        IDamageable dmg = other.GetComponentInChildren<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            Debug.LogWarning("TouchDamageDealer: hit player for " + damage);
        }
    }
}
