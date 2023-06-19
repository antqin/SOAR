using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Image button;
    private bool state; 

    private bool clicked; 


    // Start is called before the first frame update
    void Start()
    {
        button.color = Color.white;
        state = false; // off 

        clicked = false; 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    	if (state == false) { 
            button.color = Color.blue; 
        } else { 
            button.color = Color.white; 
        }
        clicked = true; 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    	button.color = Color.green; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clicked == true) { 
            clicked = false;  

            if (state == false) {
                button.color = Color.blue;
                state = true;

                if (button.name == "Button1.1") { // directionsToggleButton
                    ConfirmButton.directionState = true;
                }
                if (button.name == "Button1.2") { // annotationToggleButton
                    ConfirmButton.annotationState = true;
                } 

                if (button.name == "buttons") { 
                    Debug.Log("buttons"); 
                    ConfirmButton.annotationPhysical = true; 
                }
                if (button.name == "buttons2") { 
                    Debug.Log("buttons"); 
                    ConfirmButton.annotationBuilding = true; 
                }
                if (button.name == "buttons3") { 
                    Debug.Log("buttons"); 
                    ConfirmButton.annotationVisitor = true; 
                } 
                if (button.name == "buttons4") { 
                    ConfirmButton.annotationAll = true; 
                }

                if (button.name == "Button2.1") { // Restaurants 
                    // var foo = ConfirmButton.type_flags.ElementAt(0); 
                    // foo = true; 
                    // Debug.Log("triggered restaurants flag"); 
                    ConfirmButton.type_flags[0] = true; 

                    Debug.Log("triggered first flag"); 

                    ConfirmButton.annotationPhysical = true; 
                    // ConfirmButton.type_flags[0] = true; // 0 = Restaurants 
                    // ConfirmButton.selected_types.Add("Restaurants") 
                }
                if (button.name == "Button2.2") { // Movies 
                    // var foo = ConfirmButton.type_flags.ElementAt(1); 
                    // foo = true; 
                    // Debug.Log("triggered movies flag"); 
                    ConfirmButton.type_flags[1] = true; 
                    // ConfirmButton.type_flags[1] = true; // 1 = Movies 
                    // ConfirmButton.selected_types.Add("Movies") 
                } // 
                if (button.name == "Button2.3") { 
                    ConfirmButton.type_flags[2] = true; 
                } 
                if (button.name == "Button2.4") { // All 
                    ConfirmButton.type_flags[3] = true; 

                }



            }
            else {
                button.color = Color.white;
                state = false;


                if (button.name == "Button1.1") { // directionsToggleButton
                    ConfirmButton.directionState = false;
                }
                if (button.name == "Button1.2") { // annotationToggleButton
                    ConfirmButton.annotationState = false;
                }

                if (button.name == "buttons") { 
                    Debug.Log("buttons"); 
                    ConfirmButton.annotationPhysical = false; 
                }
                if (button.name == "buttons2") { 
                    Debug.Log("buttons"); 
                    ConfirmButton.annotationBuilding = false; 
                }
                if (button.name == "buttons3") { 
                    Debug.Log("buttons"); 
                    ConfirmButton.annotationVisitor = false; 
                } 
                if (button.name == "buttons4") { 
                    ConfirmButton.annotationAll = false; 
                }

                if (button.name == "Button2.1") { // Restaurants 
                    ConfirmButton.type_flags[0] = false; 

                    ConfirmButton.annotationPhysical = false; 
                }
                if (button.name == "Button2.2") { // Movies 
                    ConfirmButton.type_flags[1] = false; 
                } // 
                if (button.name == "Button2.3") { 
                    ConfirmButton.type_flags[2] = false; 
                } 
                if (button.name == "Button2.4") { // All 

                    ConfirmButton.type_flags[3] = false; 
                }


            } 

        } else { // clicked == true 

            if (state == false) { 
                button.color = Color.white; 
            } else { 
                button.color = Color.blue; 
            }
            
        } 
        // And activate things correspondingly from here...

        // Navigation: Block out any NAV related objects
        // Annotation: Block out any ANNOT related objects
        // Annotation types: Etc...
        // And coordinate logic b/w selecting all, and 1 thing leading to the next, and etc...



    }


    // Update is called once per frame
    void Update()
    {
        // Debug.Log(state); 
    }
}
