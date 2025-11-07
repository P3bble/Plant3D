using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 20;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Damaged, HP: " + health);

        if (health <= 0)
        {
            Debug.Log("you died:(");
        }
    }
}
