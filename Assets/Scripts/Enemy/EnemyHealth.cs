using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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

    [Header("Death/Animation")]
    [SerializeField] string dieTriggerName = "Die";
    [SerializeField] float destroyDelay = 2f; // fallback
    Animator animator;
    bool isDead;

    void Awake()
    {
        health = maxHealth;
        animator = GetComponentInChildren<Animator>(true);

        if (healthBarPrefab && headAnchor)
        {
            var bar = Instantiate(healthBarPrefab, headAnchor.position, Quaternion.identity, headAnchor);
            healthSlider = bar.GetComponentInChildren<Slider>();

            if (bar.GetComponent<Billboard>() == null)
                bar.AddComponent<Billboard>();

            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = 0;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health = Mathf.Max(0, health - Mathf.Max(0, amount));

        if (healthSlider)
            healthSlider.value = maxHealth - health;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        onAnyEnemyDied?.Invoke(this);
        DisableMovementAndCollisions();

        if (animator)
        {
            animator.ResetTrigger(dieTriggerName);
            animator.SetTrigger(dieTriggerName);
        }

        if (healthSlider) healthSlider.interactable = false;
        if (headAnchor) headAnchor.gameObject.SetActive(false);

        Debug.Log("Enemy has died");
    }


    public void OnDeathAnimationComplete()
    {
        Debug.Log("Death event fired → destroying enemy");
        Destroy(gameObject);
    }

    void DisableMovementAndCollisions()
    {
        var agent = GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.isKinematic = true;
        }

        var behaviours = GetComponents<MonoBehaviour>();
        foreach (var mb in behaviours)
        {
            if (mb == this) continue;
            mb.enabled = false;
        }

     
        foreach (var col in GetComponentsInChildren<Collider>()) col.enabled = false;
        foreach (var col2d in GetComponentsInChildren<Collider2D>()) col2d.enabled = false;
    }
}
