using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateStar : MonoBehaviour
{

      public float rotateSpeed = 50.0f;

      // Update is called once per frame
      void Update()
      {
          transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
      }
}
