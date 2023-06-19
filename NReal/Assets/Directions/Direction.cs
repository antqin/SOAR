using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Direction", menuName = "Direction")]
public class Direction : ScriptableObject
{
    public string distance;
    public string instruction;
    public Sprite image;

    public void Print() {
        Debug.Log(instruction + " in" + distance);
    }
}
