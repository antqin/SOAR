using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarReset : MonoBehaviour
{
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    public List<Waypoint> waypoints;

    void Start()
    {
        // Record the initial position and rotation of the car
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    void Update()
    {
        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset the car's position and rotation to their initial values
            transform.position = startingPosition;
            transform.rotation = startingRotation;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        }
    }
}
