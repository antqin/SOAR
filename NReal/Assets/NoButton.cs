using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NoButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject annotationDisplayObject;
    public Image button;
    private float delay = 3f; // The delay in seconds before switching to the next annotation

    private bool isDelaying = false; // A flag to indicate whether we're currently delaying
    private float delayTimer = 0f; // A timer to keep track of the delay time
    public Canvas canvas;

    void Start() {
        button.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // If we're not currently delaying, start the delay and switch to the next annotation
        if (!isDelaying)
        {
            isDelaying = true;
            delayTimer = delay;
            canvas.enabled = false;
        }
    }

    /// <summary> when pointer hover </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        button.color = Color.red;
    }

    /// <summary> when pointer exit hover </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        button.color = Color.white;
    }

    private void Update()
    {
        // If we're delaying, decrement the delay timer and switch to the next annotation when it reaches 0
        if (isDelaying)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0f) {
                canvas.enabled = true;
                // Find the AnnotationDisplay component on the Canvas
                AnnotationDisplay annotationDisplay = annotationDisplayObject.GetComponentInChildren<AnnotationDisplay>();

                // Get the next annotation in the list and set it
                Annotation[] annotations = annotationDisplay.annotations;
                Annotation curAnnotation = annotations[annotationDisplay.curIndex];
                int nextIndex = (annotationDisplay.curIndex + 1) % annotations.Length; 
                bool flip = false; 
                if (annotationDisplay.curIndex + 1 == annotations.Length) { 
                    flip = true; 
                }
                annotationDisplay.SetAnnotation(annotations[nextIndex], flip);
                isDelaying = false;
            }
        }
    }
}