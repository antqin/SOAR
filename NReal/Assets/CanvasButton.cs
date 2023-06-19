using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CanvasButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image button;
    public TMPro.TextMeshProUGUI text;

    private void Start()
    {
        button.color = Color.green;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Canvas clicked..."); 
        text.text = "Navigating..."; // Placating"; // "Navigating...";
    } 

    /// <summary> when pointer hover </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        button.color = Color.blue;
    }

    /// <summary> when pointer exit hover </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        button.color = Color.green;
    }
}
