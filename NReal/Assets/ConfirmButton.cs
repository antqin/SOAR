using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 

public class ConfirmButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{ 

    public GameObject selectionMenu; 
    public Image confirmButton; 
    public static bool confirm; // public for access 

    public Camera mainCamera; 
    public Camera leftCamera; 
    public Camera rightCamera; 

    public Camera[] cameras; 


    private bool clicked; 

    private Color baseColor; 


    public Dictionary<int, string> typeMap; 

    public static bool directionState; 
	public static bool annotationState; 
	public static bool annotationRes; 
	public static bool annotationPark; 
	// These will be updated by the component buttons... 

    public static bool annotationPhysical; 
    public static bool annotationBuilding; 
    public static bool annotationVisitor; 
    public static bool annotationAll; 

    // public Dictionary<int, string> typeMap; 
    public static List<string> selected_types; 
    // public static List<bool> type_flags; 
    public static bool[] type_flags; 

    private int cullMask; 


    // Start is called before the first frame update
    void Start()
    { 
        // confirmButton.color = Color.white; 
        baseColor = confirmButton.color; 
        // button.color = Color.white; 
        confirm = false; // off 
        mainCamera = Camera.main; 
        leftCamera = Camera.main ; // 
        rightCamera = Camera.main; // 
        cullMask = Camera.main.cullingMask; 

        cameras = Camera.allCameras; 


        clicked = false; 

        directionState = false;  
		annotationState = false; 
		annotationRes = false; 
		annotationPark = false; 

        typeMap = new Dictionary<int, string>(){{0, "Physical"}, {1, "Buildings"}, {2, "Visitor"}, {3, "All"}};  
        // type_flags = new List<bool>(new bool[typeMap.Length]); 
        type_flags = new bool[typeMap.Count]; 
        selected_types = new List<string>(); // 

        annotationPhysical = false; 
        annotationBuilding = false; 
        annotationVisitor = false; 
        annotationAll = false; 

        // foreach (bool flag in type_flags) { 
        //     Debug.Log(flag); 
        // } // 
        // foreach (string type in selected_types) { 
        //     Debug.Log(type); 
        // } // 


    } 

    public void OnPointerClick(PointerEventData eventData) 
    { 
    	confirmButton.color = Color.green; 
        clicked = true; 
    } 

    public void OnPointerEnter(PointerEventData eventData) 
    { 
    	confirmButton.color = Color.yellow; 
    } 

    public void OnPointerExit(PointerEventData eventData) 
    { 
        if (clicked == true) { 
            clicked = false; 

            if (confirm == false) { 
                confirmButton.color = Color.blue; 
                confirm = true; 

                // And now do the further logic of displaying things.  

                // 11...110...0001 
                // 11...110...0000 
                // 11...110...1000 (maybe) 
                // 11...110...1100 (maybe) 
                // //6, 7, 8, 9 

                // 00001 -> 11110 
                // & ?-> 10111 => 101110 
                // & ?-> 11101 => 101100 
                cullMask = ~(1 << 9) & ~(1 << 6) & ~(1 << 7); 

                foreach (Camera cam in Camera.allCameras) { 
                    // Debug.Log(cam.name); 
                    // Debug.Log(cam.cullingMask); 
                    cam.cullingMask = cullMask; 
                } 
                // foreach (Camera cam in Camera.allCameras) { 
                //     Debug.Log(cam.name); 
                //     Debug.Log(cam.cullingMask); 
                // } 
                // foreach (Camera cam in Camera.allCameras) { 
                //     cam.cullingMask = cullMask; 
                // } 
                // foreach (Camera cam in cameras) { 
                //     Camera currcamera = cam; // Camera.current; 
                //     Debug.Log(currcamera); 
                //     // currcamera.cullingMask = cullMask; 
                // } 

                // mainCamera.cullingMask = cullMask; 
                // leftCamera.cullingMask = cullMask; 
                // rightCamera.cullingMask = cullMask;  

                // camera.cullingMask = ~(1 << 9) & ~(1 << 6) & ~(1 << 7); 
                // Switch off layer 9 
                // 8 == selection menu layer 
                if (directionState == true) { 
                	// layer(6).on; // 6 == direction layer 
                    cullMask = cullMask | (1 << 6); 

                    // foreach (Camera cam in Camera.allCameras) { 
                    //     cam.cullingMask = cullMask; 
                    // } 

                    foreach (Camera cam in Camera.allCameras) { 
                        // Debug.Log(cam.name); 
                        // Debug.Log(cam.cullingMask); 
                        cam.cullingMask = cullMask; 
                    } 

                    // mainCamera.cullingMask = cullMask; 
                    // leftCamera.cullingMask = cullMask; 
                    // rightCamera.cullingMask = cullMask; 
                    // foreach (Camera cam in cameras) { 
                    //     Camera currcamera = Camera.current; 
                    //     currcamera.cullingMask = cullMask; 
                    // }
                	// camera.cullingMask = camera.cullingMask | (1 << 6); 
                	// Switch on layer 6 
                }
                if (annotationState == true) { 
                	// layer.annotation = on; // 7 == annotation layer 
                	cullMask = cullMask | (1 << 7); 
                    // mainCamera.cullingMask = cullMask; 

                    foreach (Camera cam in Camera.allCameras) { 
                        // Debug.Log(cam.name); 
                        // Debug.Log(cam.cullingMask); 
                        cam.cullingMask = cullMask; 
                    } 
                    
                    // foreach (Camera cam in Camera.allCameras) { 
                    //     cam.cullingMask = cullMask; 
                    // } 

                    // leftCamera.cullingMask = cullMask; 
                    // rightCamera.cullingMask = cullMask; 

                    // foreach (Camera cam in cameras) { 
                    //     Camera currcamera = Camera.current; 
                    //     currcamera.cullingMask = cullMask; 
                    // } 

                	// Switch on layer 7 
                }
                // if annot1State == true: 
                // 	layer.annot1 = on; 
                // if annot2State == true: 
                // 	layer.annot2 = on; 

                int idx = 0; 

                foreach (bool flag in type_flags) { 
                    if ((flag == true)) { //} | type_flags[3] == true) { 
                        selected_types.Add(typeMap[idx]); 
                    } 
                    idx++; 
                } // 

                // foreach (bool flag in type_flags) { 
                //     Debug.Log(flag); 
                // } // 
                // foreach (string type in selected_types) { 
                //     Debug.Log(type); 
                // } // 

                // Now we have selected_types populated 
                // Send this to AnnotationDisplay 

                // confirm = true; // 

            
            } else { // clicked == true 

                // confirmButton.color = baseColor; // Return to original color 
                // Color bCl_or = new Color(119/255f, 131/255f, 204/255f, 255/255f); 
                // confirmButton.color = bCl_or; 
                // confirmButton.color = new Color(119/255, 131/255, 204/255); 

            } 
        } 
    	
    }

    ///# 
    // Logic: 
    // Initially, only display the selection menu 
    // Then, allow the user to select and update directionState, annotationState, annot1state, annot1state 
    // Once done, user hits confirm... 
    // Then, the selection menu is deleted 
    // And certain items are rendered based on 
    // If directionState: then make that layer show up 
    // If annotationState: then make that layer show up 
    // If annot1State: then make that layer show up 
    // If annot2State: then make that layer show up 
    ///* 


    // Update is called once per frame
    void Update()
    {
        // Debug.Log(confirm); 

        // int count = Camera.allCameras.Length; 
        // Debug.Log(count); 



        // Camera[] cameras = Camera.allCameras; 
        // Debug.Log(cameras); 

        // foreach (Camera cam in Camera.allCameras) { 
        //     Debug.Log(cam.name); 
        //     Debug.Log(cam.cullingMask); 
        // } 

        // foreach (Camera cam in cameras) { 
        //     Debug.Log(cam.name); 

        //     // Camera currcamera = Camera.current; 
        //     // Camera currcamera = cam; 
        //     // Debug.Log(currcamera); 
        //     // Debug.Log(currcamera); 
        // }

        // cameras = Camera.allCameras(); 
        // Debug.Log(cameras); 
    }
}

