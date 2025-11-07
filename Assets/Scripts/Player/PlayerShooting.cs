using UnityEngine;
using UnityEngine.Events;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int ammo = 0;
    [SerializeField] private int ammoCap = 999;
    public UnityEvent<int> OnAmmoChanged;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.2f;
    private float nextFireTime = 0f;

    void Start()
    {
        OnAmmoChanged?.Invoke(ammo);
    }

    void Update()
    {
     
        bool wantsToShoot = Input.GetButton("Fire1");

        if (wantsToShoot && Time.time >= nextFireTime && ammo > 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        nextFireTime = Time.time + fireRate;

        // Spawn bullet
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Spend ammo
        ammo = Mathf.Max(0, ammo - 1);
        OnAmmoChanged?.Invoke(ammo);
    }

    public void AddAmmo(int amount)
    {
        ammo = Mathf.Clamp(ammo + amount, 0, ammoCap);
        OnAmmoChanged?.Invoke(ammo);
    }

    public bool HasAmmo() => ammo > 0;
}
