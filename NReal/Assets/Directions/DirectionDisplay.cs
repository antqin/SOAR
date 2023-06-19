using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionDisplay : MonoBehaviour
{
    public int curIndex = 0; // 
    // public int curIndex = -1; // 

    public TMPro.TextMeshProUGUI directionText;
    public TMPro.TextMeshProUGUI instructionText;
    public Image artworkImage;
    
    // To be populated by another script, DirectionsExample.cs:
    public List<string> instructions = new List<string>();
    public List<string> modifiers = new List<string>();

    // Navigation images
    public Sprite uturn;
    public Sprite sharpRight;
    public Sprite right;
    public Sprite slightRight;
    public Sprite straight;
    public Sprite slightLeft;
    public Sprite left;
    public Sprite sharpLeft;
    public Sprite depart;
    public Sprite arrive;

    public Canvas likedAnnotations;
    public Canvas directions;
    public Canvas socialAnnotation;

    public void SetDirection()
    {
        Debug.Log("in directions"); 
        Debug.Log(curIndex); // 
        Debug.Log(instructions.Count); // 
        // int ctf = 2; 


        // if (curIndex >= ctf) { // instructions.Count) {. // 
        if (curIndex >= instructions.Count) { 
            likedAnnotations.enabled = true;
            directions.enabled = false;
            socialAnnotation.enabled = false;
            return;
        }
        string dir = modifiers[curIndex]; // e.g. "right"
        directionText.text = dir;
        instructionText.text = instructions[curIndex];
        if (string.IsNullOrEmpty(dir)) {
            if (curIndex == 0) {
                artworkImage.sprite = depart;
            } else {
                artworkImage.sprite = arrive;
            }
        } else if (dir == "uturn") {
            artworkImage.sprite = uturn; 
        } else if (dir == "sharp right") {
            artworkImage.sprite = sharpRight; 
        } else if (dir == "right") {
            artworkImage.sprite = right; 
        } else if (dir == "slight right") {
            artworkImage.sprite = slightRight; 
        } else if (dir == "straight") {
            artworkImage.sprite = straight; 
        } else if (dir == "slight left") {
            artworkImage.sprite = slightLeft; 
        } else if (dir == "left") {
            artworkImage.sprite = left; 
        } else if (dir == "sharp left") {
            artworkImage.sprite = sharpLeft; 
        }
    }
}
