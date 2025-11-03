using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private PlantWaveSpawner spawner;

    [Header("TMP Labels")]
    [SerializeField] private TextMeshProUGUI waveLabel;
    [SerializeField] private TextMeshProUGUI timeLeftLabel;
    [SerializeField] private TextMeshProUGUI cooldownLabel;
    [SerializeField] private TextMeshProUGUI aliveLabel;
    [SerializeField] private TextMeshProUGUI remainingLabel;

    void Update()
    {
        if (!spawner) return;

        // Wave index & name
        string waveName = string.IsNullOrEmpty(spawner.CurrentWaveName) ? $"Wave {spawner.CurrentWaveNumber}/{spawner.TotalWaves}"
                                                                        : $"{spawner.CurrentWaveName} ({spawner.CurrentWaveNumber}/{spawner.TotalWaves})";
        if (waveLabel) waveLabel.text = waveName;

        // Time left during wave
        if (timeLeftLabel)
        {
            if (spawner.State == PlantWaveSpawner.SpawnerState.InWave)
                timeLeftLabel.text = $"Wave Time: {Mathf.CeilToInt(spawner.WaveTimeLeft)}s";
            else
                timeLeftLabel.text = $"Wave Time: —";
        }

        // Cooldown
        if (cooldownLabel)
        {
            if (spawner.State == PlantWaveSpawner.SpawnerState.Cooldown)
                cooldownLabel.text = $"Next Wave In: {Mathf.CeilToInt(spawner.CooldownTimeLeft)}s";
            else if (spawner.State == PlantWaveSpawner.SpawnerState.Finished)
                cooldownLabel.text = "All Waves Complete";
            else
                cooldownLabel.text = "Next Wave In: —";
        }

        // Alive plants
        if (aliveLabel) aliveLabel.text = $"Alive: {spawner.Alive}";

        // Remaining spawns this wave
        if (remainingLabel) remainingLabel.text = $"Remaining To Spawn: {spawner.RemainingToSpawnThisWave}";
    }
}
