using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pad : MonoBehaviour
{
    [SerializeField] string targetTag = "Player";
    [SerializeField] float launchForce = 12f;
    [SerializeField] AudioClip bounceSfx;

    AudioSource audioSource;

    void Awake()
    {
        var col = GetComponent<Collider>();


        audioSource = GetComponent<AudioSource>();
        if (!audioSource && bounceSfx)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (!rb) return;


        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;
        rb.linearVelocity = vel;

        rb.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);

        if (audioSource && bounceSfx)
            audioSource.PlayOneShot(bounceSfx);
    }
}
