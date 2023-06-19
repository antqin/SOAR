using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera virtualCamera;
    public Camera povCamera;

    void Start()
    {
        // Set the virtual camera as the default active camera
        virtualCamera.enabled = true;
        povCamera.enabled = false;
    }

    void Update()
    {
        // Check if the "C" key is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Toggle the enabled state of the cameras
            virtualCamera.enabled = !virtualCamera.enabled;
            povCamera.enabled = !povCamera.enabled;
        }
    }
}
