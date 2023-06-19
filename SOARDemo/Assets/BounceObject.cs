using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceObject : MonoBehaviour
{
    public float bounceHeight = 0.15f;
    public float bounceSpeed = 4.0f;

    private float startY;
    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startY + (Mathf.Sin(Time.time * bounceSpeed) * bounceHeight);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
