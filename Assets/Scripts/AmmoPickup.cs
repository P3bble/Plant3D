using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int amount = 15;
    [SerializeField] private AudioClip pickupSfx;

    private void OnTriggerEnter(Collider other)
    {
        var shooting = other.GetComponentInParent<PlayerShooting>();
        if (shooting == null) shooting = other.GetComponent<PlayerShooting>();

        if (shooting != null)
        {
            shooting.AddAmmo(amount);
            if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
            Destroy(gameObject);
        }
    }
}
