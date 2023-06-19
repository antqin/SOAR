using UnityEngine;
using System.Collections.Generic;

public class DrawPath : MonoBehaviour
{
    public Transform car;
    public List<Transform> waypoints = new List<Transform>();
    public Material pathMaterial;

    private LineRenderer lineRenderer;
    private int currentWaypointIndex = 1;
    private int leftoverWaypoints;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = pathMaterial;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.positionCount = waypoints.Count;
        leftoverWaypoints = waypoints.Count;

        for (int i = 0; i < waypoints.Count; i++)
        {
            lineRenderer.SetPosition(i, waypoints[i].position + Vector3.up * 0.1f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // check if "r" key is pressed
        {
            // reset line renderer to initial positions of waypoints
            for (int i = 0; i < waypoints.Count; i++)
            {
                lineRenderer.SetPosition(i, waypoints[i].position + Vector3.up * 0.1f);
            }

            // reset leftoverWaypoints variable
            leftoverWaypoints = waypoints.Count;
        }

        if (leftoverWaypoints > 1 && Vector3.Distance(car.position, lineRenderer.GetPosition(currentWaypointIndex)) < 4f)
        {
           leftoverWaypoints--;
           for (int i = 1; i < leftoverWaypoints; i++) {
                lineRenderer.SetPosition(i, lineRenderer.GetPosition(i + 1));
           }
        }

        if (leftoverWaypoints != 1) {
            // update the first point of the line renderer to the position of the car
            lineRenderer.SetPosition(0, car.position + Vector3.up * 0.1f);
        }

        if (leftoverWaypoints == 1)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
