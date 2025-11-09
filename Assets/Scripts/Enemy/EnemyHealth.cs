using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public static event System.Action<EnemyHealth> onAnyEnemyDied;

    [Header("Health")]
    [SerializeField] int maxHealth = 30;
    int health;

    [Header("Health Bar")]
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] Transform headAnchor;
    Slider healthSlider;

    void Awake()
    {
        health = maxHealth;

        if (healthBarPrefab && headAnchor)
        {
            var bar = Instantiate(healthBarPrefab, headAnchor.position, Quaternion.identity, headAnchor);
            healthSlider = bar.GetComponentInChildren<Slider>();

            if (bar.GetComponent<Billboard>() == null)
                bar.AddComponent<Billboard>();

         
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = 0; // full at start
        }
    }

    public void TakeDamage(int amount)
    {
        health = Mathf.Max(0, health - Mathf.Max(0, amount));

        if (healthSlider)
            healthSlider.value = maxHealth - health;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        onAnyEnemyDied?.Invoke(this);
        Destroy(gameObject);
        Debug.Log("Enemy has died");
    }
}
