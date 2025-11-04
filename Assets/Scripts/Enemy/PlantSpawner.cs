using System.Collections;
using UnityEngine;

public class PlantWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave";
        public float durationSeconds = 20f;   // Wave time
        public int spawnCount = 10;           // Enimies that will spawn(how many)
        public GameObject[] plantPrefabs;     // which enimies can spawn?
    }

    public enum SpawnerState { Idle, InWave, Cooldown, Finished }

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float cooldownBetweenWaves = 8f;

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxAlive = 20;

    [Header("Wave End Options")]
    [SerializeField] private bool clearOnCooldown = true; // kill all at wave end

    // Runtime state
    private int currentWaveIndex = -1;
    private int spawnedThisWave = 0;
    private int alive = 0;

    private float waveTimeLeft = 0f;
    private float cooldownTimeLeft = 0f;
    private float spawnInterval = 0f; 
    private float spawnTimer = 0f;

    private SpawnerState state = SpawnerState.Idle;

    // Public read-only props for UI
    public SpawnerState State => state;
    public int CurrentWaveNumber => Mathf.Clamp(currentWaveIndex + 1, 0, waves.Length);
    public int TotalWaves => waves.Length;
    public float WaveTimeLeft => Mathf.Max(0f, waveTimeLeft);
    public float CooldownTimeLeft => Mathf.Max(0f, cooldownTimeLeft);
    public int Alive => alive;
    public int SpawnedThisWave => spawnedThisWave;
    public int RemainingToSpawnThisWave => Mathf.Max(0, (IsValidWave ? waves[currentWaveIndex].spawnCount : 0) - spawnedThisWave);
    public string CurrentWaveName => IsValidWave ? waves[currentWaveIndex].waveName : "";
    private bool IsValidWave => currentWaveIndex >= 0 && currentWaveIndex < waves.Length;

    void OnEnable()
    {
        // Requires your EnemyHealth to have: public static System.Action<EnemyHealth> onAnyEnemyDied;
        EnemyHealth.onAnyEnemyDied += OnAnyEnemyDied;
    }

    void OnDisable()
    {
        EnemyHealth.onAnyEnemyDied -= OnAnyEnemyDied;
    }

    void Start()
    {
        if (waves == null || waves.Length == 0)
        {
            Debug.LogWarning("PlantWaveSpawner: No waves configured.");
            state = SpawnerState.Finished;
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("PlantWaveSpawner: No spawn points assigned.");
        }

        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        // Start immediately with Wave 1 (no pre-wave countdown here)
        for (currentWaveIndex = 0; currentWaveIndex < waves.Length; currentWaveIndex++)
        {
            BeginWave(waves[currentWaveIndex]);

            while (state == SpawnerState.InWave)
            {
                TickWave();
                yield return null;
            }

            // Wave ended: clear all enemies for rest period
            if (clearOnCooldown)
            {
                WipeAllEnemies();
                alive = 0; // keep counter in sync
            }

            // Cooldown between waves (skip cooldown after last wave)
            if (currentWaveIndex < waves.Length - 1)
            {
                state = SpawnerState.Cooldown;
                cooldownTimeLeft = cooldownBetweenWaves;
                while (cooldownTimeLeft > 0f)
                {
                    cooldownTimeLeft -= Time.deltaTime;
                    yield return null;
                }
            }
        }

        state = SpawnerState.Finished;
    }

    private void BeginWave(Wave w)
    {
        state = SpawnerState.InWave;
        spawnedThisWave = 0;
        waveTimeLeft = Mathf.Max(0.01f, w.durationSeconds);

        int count = Mathf.Max(0, w.spawnCount);
        spawnInterval = (count > 0) ? (w.durationSeconds / count) : w.durationSeconds + 1f;
        spawnTimer = 0f;
    }

    private void TickWave()
    {
        if (!IsValidWave) { state = SpawnerState.Finished; return; }

        Wave w = waves[currentWaveIndex];

        waveTimeLeft -= Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // Spawn during the wave, respecting maxAlive and total count
        if (spawnedThisWave < w.spawnCount && spawnTimer >= spawnInterval)
        {
            TrySpawnOne(w);
            spawnTimer = 0f;
        }

        // End the wave when the timer runs out (spawns stop; plants still alive can remain)
        if (waveTimeLeft <= 0f)
        {
            state = SpawnerState.Idle; // Run() will handle cooldown + wipe
        }
    }

    private void TrySpawnOne(Wave w)
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (w.plantPrefabs == null || w.plantPrefabs.Length == 0) return;
        if (alive >= maxAlive) return;

        Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = w.plantPrefabs[Random.Range(0, w.plantPrefabs.Length)];
        if (!prefab) return;

        Instantiate(prefab, p.position, p.rotation);
        spawnedThisWave++;
        alive++;
    }

    private void OnAnyEnemyDied(EnemyHealth h)
    {
        alive = Mathf.Max(0, alive - 1);
    }

    // Kill everything that uses EnemyHealth (so normal death events/VFX still run)
    private void WipeAllEnemies()
    {
        var enemies = FindObjectsOfType<EnemyHealth>();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].TakeDamage(int.MaxValue);
        }
    }
}
