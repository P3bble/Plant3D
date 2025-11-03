using UnityEngine;
using TMPro; // make sure you have TextMeshPro in your project

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    public void UpdateAmmoText(int currentAmmo)
    {
        ammoText.text = "Ammo: " + currentAmmo;
    }
}
