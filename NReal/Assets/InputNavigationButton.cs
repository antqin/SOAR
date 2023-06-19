using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputNavigationButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image button;
    public TMPro.TextMeshProUGUI text;
    public Canvas getDirections; // 
    public Canvas navigation;
    public Canvas socialAnnotation;
    public DirectionDisplay directionDisplay; // 

    private bool confirmRepopulate = false; 

    private void Start()
    {
        Debug.Log("in input"); 
        button.color = Color.green;
        navigation.enabled = false; 
        socialAnnotation.enabled = false; 
        Debug.Log("start"); 
        if (ConfirmButton.directionState == false && ConfirmButton.annotationState == true) { 
            Debug.Log("show"); 
            socialAnnotation.enabled = true; 
        } 
        getDirections.enabled = true; // 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("clicked"); 
        Debug.Log("navigate clicked"); 
        navigation.enabled = true;
        socialAnnotation.enabled = true;
        getDirections.enabled = false; // 
        // text.text = "Placating..."; 
        text.text = "Navigating..."; // Placaters'ing"; 
        // text.text = "Navigating..."; // 
        directionDisplay.SetDirection(); // 
    }

    /// <summary> when pointer hover </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("entered"); 
        button.color = Color.blue; 
    }

    /// <summary> when pointer exit hover </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("exited"); 
        button.color = Color.green;
    }

    private void Update() { 
        if (confirmRepopulate == false) { 
            if (ConfirmButton.confirm == true) { 
                Start(); // Start2() 
                // run Start again 
                confirmRepopulate = true; 
            } 
        } 
    } 
}