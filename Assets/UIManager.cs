using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    public void UpdateAmmoText(int currentAmmo)
    {
        ammoText.text = "Ammo: " + currentAmmo;
    }
}
