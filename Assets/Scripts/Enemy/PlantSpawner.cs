using System.Collections;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxAlive = 20;

    private int alive;

    void OnEnable() => EnemyHealth.onAnyEnemyDied += HandleDied;
    void OnDisable() => EnemyHealth.onAnyEnemyDied -= HandleDied;

    void Start() => StartCoroutine(SpawnLoop());

    IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnInterval);
        while (true)
        {
            if (alive < maxAlive && plantPrefab && spawnPoints.Length > 0)
            {
                Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(plantPrefab, p.position, p.rotation);
                alive++;
            }
            yield return wait;
        }
    }

    private void HandleDied(EnemyHealth h) => alive = Mathf.Max(0, alive - 1);
}
