using UnityEngine;

public class Waypoint : MonoBehaviour {
    public float radius;

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}