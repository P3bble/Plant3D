using UnityEngine;

public class PlantSimpleChase : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] string targetTag = "Player";

    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float stopDistance = 1.2f;   // how close it’s allowed to get to the player

    [Header("Attack")]
    [SerializeField] float attackRange = 1.7f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] int damage = 1;

    Transform target;
    IDamageable damageTarget;
    float nextAttackTime;

    Collider[] myColliders;

    void Awake()
    {
        // get ALL colliders on this enemy (root + children)
        myColliders = GetComponentsInChildren<Collider>();

        GameObject found = GameObject.FindGameObjectWithTag(targetTag);
        if (found)
        {
            target = found.transform;
            damageTarget = found.GetComponentInChildren<IDamageable>();
        }
    }

    void Update()
    {
        if (target == null || damageTarget == null) return;

        // flat direction to player
        Vector3 to = target.position - transform.position;
        to.y = 0f;
        float d = to.magnitude;

        // rotate toward player
        if (to.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // move toward player until we're at stopDistance (and no closer)
        if (d > stopDistance)
        {
            Vector3 dir = to.normalized;
            float maxStep = moveSpeed * Time.deltaTime;

            // only move as much as needed to reach stopDistance, not overshoot
            float distanceToMove = Mathf.Min(maxStep, d - stopDistance);

            Vector3 step = dir * distanceToMove;
            transform.position += step;
        }

        // snap to ground so we don't float / sink
        SnapToGround();

        // attack if close enough and cooldown ready
        if (d <= attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            Debug.Log($"Enemy hit player at distance {d}");
            damageTarget.TakeDamage(damage);
        }
    }

    void SnapToGround()
    {
        // cast a ray down from a bit above the enemy
        Vector3 origin = transform.position + Vector3.up * 2f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    // ---- Ignore items so we can walk through loot ----

    void IgnoreItemCollider(Collider other)
    {
        if (!other.CompareTag("Item")) return;

        // ignore collisions between ALL our colliders and the item collider
        foreach (var col in myColliders)
        {
            if (col != null)
                Physics.IgnoreCollision(col, other, true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        IgnoreItemCollider(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        IgnoreItemCollider(other);
    }
}
