using UnityEngine;

public class CarBrake : MonoBehaviour
{
    private Rigidbody carRigidbody;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the "Space" key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Reset the car's position and rotation
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }
    }
}
