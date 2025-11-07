using UnityEngine;

public class PlayerTouchDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GetComponent<PlayerHealth>().TakeDamage(damage);
            Debug.LogWarning("DAMAGE TAKEN");
        }
    }
}
