using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public float delayTime = 10f; // The delay time in seconds before the canvas appears
    private Canvas canvas; // The Canvas component attached to the game object

    private void Start()
    {
        canvas = GetComponent<Canvas>(); // Get the Canvas component attached to the game object
        canvas.enabled = false; // Hide the canvas when the game starts
        Invoke("ShowCanvas", delayTime); // Call the ShowCanvas method after the specified delay time
    }

    private void ShowCanvas()
    {
        canvas.enabled = true; // Show the canvas after the delay time has elapsed
    }
}
