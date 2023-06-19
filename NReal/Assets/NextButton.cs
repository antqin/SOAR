using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NextButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject annotationDisplayObject;
    public Image button;

    private int curIndex = 0;
    public Canvas canvas;

    public LikeButton likeButton; // Reference to the LikeButton script
    public TMPro.TextMeshProUGUI text; // Navigate text

    void Start() {
        canvas.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Find the AnnotationDisplay component on the Canvas
        AnnotationDisplay annotationDisplay = annotationDisplayObject.GetComponentInChildren<AnnotationDisplay>();
        int nextIndex = (curIndex + 1) % likeButton.likedAnnotations.Count;
        annotationDisplay.SetAnnotation(likeButton.likedAnnotations[curIndex], false);
        text.text = "Navigate";
        curIndex = nextIndex;
    }

    /// <summary> when pointer hover </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        button.color = Color.blue;
    }

    /// <summary> when pointer exit hover </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        button.color = Color.black;
    }
}