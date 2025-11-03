using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static System.Action<EnemyHealth> onAnyEnemyDied;

    [SerializeField] private int maxHealth = 30;
    private int health;

    void Awake() => health = maxHealth;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    private void Die()
    {
        onAnyEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }
}
