using UnityEngine;

public class DiamondPickup : MonoBehaviour
{
    public int value = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DiamondManager.Instance.AddMoney(value);
            Destroy(gameObject);
        }
    }
}
// working