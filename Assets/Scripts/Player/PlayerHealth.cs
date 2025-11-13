using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int health = 20;

    [Header("Audio")]
    [SerializeField] AudioClip hpLoss;
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (hpLoss != null && audioSource != null)
            audioSource.PlayOneShot(hpLoss);

        Debug.Log("Damaged, HP: " + health);

        if (health <= 0)
        {
            Debug.Log("you died :(");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
