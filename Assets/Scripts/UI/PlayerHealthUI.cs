using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;

    private int maxHealth;

    void Start()
    {
        maxHealth = playerHealth.health;
    }

    void Update()
    {
        float fill = Mathf.Clamp01(playerHealth.health / (float)maxHealth);
        fillImage.fillAmount = fill; // filling HP bar
    }
}
