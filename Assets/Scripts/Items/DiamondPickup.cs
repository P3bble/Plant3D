using UnityEngine;

public class DiamondPickup : MonoBehaviour
{
    [SerializeField] private int diamondValue = 1;
    [SerializeField] private AudioClip pickupSound;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming there's a GameManager class that handles the player's diamond count
         //   GameManager.Instance.AddDiamonds(diamondValue);
            // Play pickup sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            // Destroy the diamond object
            Destroy(gameObject);
        }
    }


}
