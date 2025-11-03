using UnityEngine;

public class PlantSimpleChase : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float speed = 3f;
    [SerializeField] private float aggroRange = 15f;
    [SerializeField] private float stopDistance = 1.5f;

    private Transform target;

    void Start()
    {
        GameObject found = GameObject.FindGameObjectWithTag(targetTag);
        if (found != null) target = found.transform;
        else Debug.LogWarning($"{name}: No GameObject found with tag '{targetTag}'");
    }

    void Update()
    {
        if (!target) return;

        Vector3 to = target.position - transform.position;
        float d = to.magnitude;

        if (d > aggroRange) return;

        if (d > stopDistance)
        {
            Vector3 dir = to.normalized;
            transform.position += dir * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir, Vector3.up),
                10f * Time.deltaTime
            );
        }
    }
}
