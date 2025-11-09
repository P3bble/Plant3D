using UnityEngine;

public class PlantSimpleChase : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] string targetTag = "Player";
    [SerializeField] float aggroRange = 15f;
    [SerializeField] float stopDistance = 1.5f;
    [SerializeField] float aggroStickTime = 3f;

    [Header("Movement")]
    [SerializeField] float speed = 3f;
    [SerializeField] float turnSpeed = 10f;

    Transform target;
    float lastAggroTime;

    Rigidbody rb; //freeze physics rotation

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb) rb.freezeRotation = true;

        GameObject found = GameObject.FindGameObjectWithTag(targetTag);
        if (found) target = found.transform;
    }

    void Update()
    {
        if (!target) return;

        // don’t drift up/down
        Vector3 to = target.position - transform.position;
        to.y = 0f;
        float d = to.magnitude;

     
        if (d <= aggroRange) lastAggroTime = Time.time;

        bool hasAggro = Time.time < lastAggroTime + aggroStickTime;

        if (!hasAggro) return;

        if (to.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        if (d > stopDistance)
        {
            Vector3 step = transform.forward * speed * Time.deltaTime;
            // keep y fixed
            step.y = 0f;
            transform.position += step;
        }
    }

    public void OnHitAggro()
    {
        lastAggroTime = Time.time;
    }
}
