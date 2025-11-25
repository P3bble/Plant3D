 using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void OnEnable()
    {
        if (rb != null)
            rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
{

    if (!collision.collider.CompareTag("Enemy"))
    {
        Destroy(gameObject);
        return;
    }

    var dmg = collision.collider.GetComponentInParent<IDamageable>();
    if (dmg != null)
        dmg.TakeDamage(damage);

    Destroy(gameObject);
}

}
