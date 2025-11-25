using UnityEngine;
using UnityEngine.UI;

public class PlantHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 40;
    [SerializeField] Slider hpSlider;
    [SerializeField] GameObject gameOverScreen;

    int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;

        if (hpSlider == null)
            hpSlider = GetComponentInChildren<Slider>();

        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHealth;
            hpSlider.wholeNumbers = true;
            UpdateBar();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateBar();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateBar()
    {
        if (hpSlider == null) return;

        int damageTaken = maxHealth - currentHealth;
        hpSlider.value = damageTaken;
    }

    void Die()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}
