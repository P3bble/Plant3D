using UnityEngine;

public class PlantSimpleChase : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] string plantTag = "Plant";
    [SerializeField] float playerAggroRange = 4f;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float stopDistance = 1.2f;

    [Header("Attack")]
    [SerializeField] float attackRange = 1.7f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] int damage = 1;

    Transform playerTarget;
    Transform plantTarget;
    IDamageable playerDamageTarget;
    IDamageable plantDamageTarget;

    Transform currentTarget;
    IDamageable currentDamageTarget;

    float nextAttackTime;
    Collider[] myColliders;

    void Awake()
    {
        myColliders = GetComponentsInChildren<Collider>();

        GameObject foundPlayer = GameObject.FindGameObjectWithTag(playerTag);
        if (foundPlayer)
        {
            playerTarget = foundPlayer.transform;
            playerDamageTarget = foundPlayer.GetComponentInChildren<IDamageable>();
        }

        GameObject foundPlant = GameObject.FindGameObjectWithTag(plantTag);
        if (foundPlant)
        {
            plantTarget = foundPlant.transform;
            plantDamageTarget = foundPlant.GetComponentInChildren<IDamageable>();
        }
    }

    void Update()
    {
        SnapToGround();
        ChooseTarget();

        if (currentTarget == null) return;

        Vector3 to = currentTarget.position - transform.position;
        to.y = 0f;
        float d = to.magnitude;

        if (to.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        if (d > stopDistance)
        {
            Vector3 dir = to.normalized;
            float maxStep = moveSpeed * Time.deltaTime;
            float distanceToMove = Mathf.Min(maxStep, d - stopDistance);
            Vector3 step = dir * distanceToMove;
            transform.position += step;
        }

        if (d <= attackRange && Time.time >= nextAttackTime && currentDamageTarget != null)
        {
            nextAttackTime = Time.time + attackCooldown;
            currentDamageTarget.TakeDamage(damage);
        }
    }

    void ChooseTarget()
    {
        currentTarget = null;
        currentDamageTarget = null;

        bool hasPlayer = playerTarget != null;
        bool hasPlant = plantTarget != null;

        if (!hasPlayer && !hasPlant) return;

        if (hasPlayer)
        {
            float playerDist = Vector3.Distance(transform.position, playerTarget.position);
            if (playerDist <= playerAggroRange)
            {
                currentTarget = playerTarget;
                currentDamageTarget = playerDamageTarget;
                return;
            }
        }

        if (hasPlant)
        {
            currentTarget = plantTarget;
            currentDamageTarget = plantDamageTarget;
        }
    }

    void SnapToGround()
    {
        Vector3 origin = transform.position + Vector3.up * 2f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f))
        {
            if (hit.collider.CompareTag(playerTag) || hit.collider.CompareTag(plantTag))
                return;

            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    void IgnoreItemCollider(Collider other)
    {
        if (!other.CompareTag("Item")) return;

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
