using UnityEngine;

public class ObjectSpin : MonoBehaviour
{
    public float rotationSpeed = 45f; // Degrees per second


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
       // transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.fwd, rotationSpeed * Time.deltaTime);
    }
}
