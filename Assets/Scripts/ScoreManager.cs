using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public event Action<int> OnScoreChanged;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        EnemyHealth.onAnyEnemyDied += OnEnemyDied;
    }

    void OnDisable()
    {
        EnemyHealth.onAnyEnemyDied -= OnEnemyDied;
    }

    void OnEnemyDied(EnemyHealth enemy)
    {
        Add(1);
    }

    public void Add(int points)
    {
        Score += points;
        OnScoreChanged?.Invoke(Score); // tell UI
        // debugging is working
        Debug.Log($"Score: {Score}");
    }

    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score); // refresh UI
    }
}
