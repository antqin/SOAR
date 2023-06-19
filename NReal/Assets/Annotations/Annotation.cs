using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Annotation", menuName = "Annotation")]
public class Annotation : ScriptableObject
{
    public string name;
    public string description;
    public string review;
    public string address;
    public Sprite artwork;

    public string type; // 

    public void Print() {
        Debug.Log(name + ": " + description + " the address: " + address); 
      	// Here: ... // Perhaps check if it's here 
    }
}
