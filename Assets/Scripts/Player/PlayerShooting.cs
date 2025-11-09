using UnityEngine;
using UnityEngine.Events;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] int ammo = 0;
    [SerializeField] int ammoCap = 999;
    public UnityEvent<int> OnAmmoChanged;

    [Header("Shooting")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireRate = 0.2f;
    float nextFireAt;

    [Header("Audio")]
    AudioSource audioSource;
    [SerializeField] AudioClip shooting;
    [SerializeField] AudioClip cantFire;
    [SerializeField] AudioClip pickup;


    void Start()
    {
        OnAmmoChanged?.Invoke(ammo);
    }
   
 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
      
        if (!Input.GetButton("Fire1")) return;

       
        if (Time.time < nextFireAt) return;

        if (ammo > 0)
        {
            Fire();
        }
        else
        {
            
            nextFireAt = Time.time + fireRate;
            PlayOneShot(cantFire);
        }
    }

    void Fire()
    {
        nextFireAt = Time.time + fireRate;

        // bullet
        if (bulletPrefab && firePoint)
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // spend ammo UI
        ammo = Mathf.Max(0, ammo - 1);
        OnAmmoChanged?.Invoke(ammo);

        PlayOneShot(shooting);
    }

    public void AddAmmo(int amount)
    {
        ammo = Mathf.Clamp(ammo + Mathf.Max(0, amount), 0, ammoCap);
        OnAmmoChanged?.Invoke(ammo);
        PlayOneShot(pickup);
    }

    public bool HasAmmo() => ammo > 0;

    void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
