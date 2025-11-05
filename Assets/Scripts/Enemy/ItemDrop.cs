using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private GameObject[] dropItems; // assign prefabs in Inspector
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.3f; // 30% chance to drop something

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        EnemyHealth.onAnyEnemyDied += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        EnemyHealth.onAnyEnemyDied -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath(EnemyHealth deadEnemy)
    {
        // only drop if *this* enemy died
        if (deadEnemy == enemyHealth)
        {
            TryDropItem();
        }
    }

    private void TryDropItem()
    {
        if (Random.value <= dropChance && dropItems.Length > 0)
        {
            // pick random item from array
            GameObject itemToDrop = dropItems[Random.Range(0, dropItems.Length)];

            // spawn at enemy’s death position
            Instantiate(itemToDrop, transform.position, Quaternion.identity);

            Debug.Log($"Item dropped from {name}!");
        }
    }
}
