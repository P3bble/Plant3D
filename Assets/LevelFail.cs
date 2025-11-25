using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFail : MonoBehaviour
{
    [SerializeField] PlantHealth plant;
    [SerializeField] GameObject gameOverScreen;

    public void Retry()
    {
        Time.timeScale = 1f;

        if (plant != null)
            plant.ResetHealth();

        // destroy all existing enemies
        var enemies = FindObjectsOfType<EnemyHealth>();
        for (int i = 0; i < enemies.Length; i++)
            Destroy(enemies[i].gameObject);

        // reset spawner & waves back to start
        var spawner = FindObjectOfType<PlantWaveSpawner>();
        if (spawner != null)
            spawner.ResetSpawner();

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
