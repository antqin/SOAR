using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEditor; 
using System.Linq; // 

public class AnnotationDisplay : MonoBehaviour
{

    // public Annotation[] initial_annotations; 



    public Annotation[] annotations; // Annotation bank 
    public int curIndex = 0; 

    // public Annotation[] annots;  



    public List<Annotation> annotation_array = new List<Annotation>(); 

    // public List<Annotation> annot_load; 

    // private List<string> physical_annotations = ["Maples Pavilion", "Stanford Stadium", "Taube Center"]; 
    // private List<string> building_annotations = ["Graduate School of Business", "Visitor Center"]; 
    // private List<string> visitor_annotations = ["Cantor Arts Center"]; 

    private Dictionary<string, List<string>> annotation_map = new Dictionary<string, List<string>>()
    { 
        {"Physical", new List<string>{"Maples Pavilion", "Stanford Stadium", "Taube Center"}}, 
        {"Buildings", new List<string>{"Graduate School of Business", "Visitor Center"}}, 
        {"Visitor", new List<string>{"Cantor Arts Center"}} 
    }; 

    private Dictionary<int, string> typeMap2 = new Dictionary<int, string>(){{0, "Physical"}, {1, "Buildings"}, {2, "Visitor"}, {3, "All"}};  


    private bool confirmRepopulate = false; 
    // anntations = // populate from folder 
    // for annotation in /Annotations 

    // var guids = AssetDatabase.FindAssets("t:Annotation"); 
    // var annotations = new Annotation[guids.length]; 

    // Only populate annotations of a particular layer into annotations 
    // OR, just flick through until we get annotations of a particular type, 
    // skipping over annotations that aren't of that type 
    // EFFECTIVELY: Only showing annotations of the selected type... 

    // Here! 

    // Perhaps?? 
    // Or where those annotations are in the right layer 
    // selected_types = ["Restaurants", "Movies"] // Not Parks 
    // foreach type in selected_types: 
    //     foreach annotation in folder=restaurants: 
    //         annotations.append(annotation); 

    // Now we have a populated annotations array... 


    public Canvas likedAnnotations; 
    public Canvas socialAnnotation;
    

    public TMPro.TextMeshProUGUI locationText;
    public TMPro.TextMeshProUGUI descriptionText;
    public Image artworkImage;
    public TMPro.TextMeshProUGUI reviewText;
    public TMPro.TextMeshProUGUI addressText;

    void Start () { 



        Debug.Log("NUMS: "); 
        Debug.Log($"Last: {annotations.Length}"); 

        // foreach(Annotation annot in annotations) { 
        //     // annotation_array.Add(annot); // 
        //     // if(ConfirmButton.selected_types.Contains(annot.type)) { 
        //     //     annotation_array.Add(annot); 
        //     // } 
        //     if(ConfirmButton.selected_types.Contains(annot.type)) { 
        //         annotation_array.Add(annot); 
        //     }
        // } 

        // annotations = annotation_array.ToArray(); 

        Debug.Log("NUMS F: "); 
        Debug.Log($"Last: {annotations.Length}"); // 


        // Debug.Log("NUMS 1: "); 
        // Debug.Log($"First: {initial_annotations.Length}"); 
        // Debug.Log($"Middle: {annotation_array.Count}"); 
        // Debug.Log($"Last: {annotations.Length}"); 


        // Debug.Log("here"); 
        // foreach(string type in ConfirmButton.selected_types) { 
        //     Debug.Log(type); 
        // } // 

        // annotation_array = initial_annotations.ToList(); 


        /* 
        foreach(Annotation annot in initial_annotations) { 
            if(ConfirmButton.selected_types.Contains(annot.type)) { 
                annotation_array.Add(annot); 
            } 
        } 
        */ 

        // annotations = annotation_array.ToArray(); // To not redo syntax 

        // annotations = initial_annotations; 

        // foreach(Annotation annot in initial_annotations) { 
        //     annotation_array.Add(annot); 
        // } // 

        // annotations = annotation_array.ToArray(); // 


        // Debug.Log("NUMS 2: "); 
        // Debug.Log($"First: {initial_annotations.Length}"); 
        // Debug.Log($"Middle: {annotation_array.Count}"); 
        // Debug.Log($"Last: {annotations.Length}"); 


        

        // // /* 

        // // string[] guids = AssetDatabase.FindAssets("t:Annotation"); 
        // // foreach (string guid in guids) { 
        // //     string path = AssetDatabase.GUIDToAssetPath(guid); 
        // //     Debug.Log(AssetDatabase.LoadAssetAtPath<Annotation>(path)); 
        // // } // 

        // // var annots = new Annotation[guids.length]; 

        // // Annotation[] annotations_array; 
        // // string[] annotations_guids; 
        // // types = [x, y, z] 
        // // foreach(string type in selected_types) { 
        // //     folder_path = string("{}_Annotations", type); 
        // //     string[] guids = AssetDatabase.FindAssets("t:Annotation", folder_path); 
        // //     annoatation_guids.add(guids); 
        // // } 
        // // foreach(string guid in guids) { 
        // //     annotations_array.add(AssetDatabase.LoadAssetAtPath<Annotation>(path)); 
        // // } 

        // // ConfirmButton.selected_types; // Populated already... 

        // // List<string> selected_types = new List<string>{"Restaurants", "Movies"}; //  


        // Debug.Log("NUMS 1: "); 
        // Debug.Log($"Last: {annotations.Length}"); 
        // Debug.Log($"Middle: {annotation_array.Count}"); 
        // Debug.Log($"First: {initial_annotations.Length}"); 

        // // annotation_array = initial_annotations.ToList(); 


        // Debug.Log("here"); 
        // foreach(string type in ConfirmButton.selected_types) { 
        //     Debug.Log(type); 
        // } 


        // // annot_load = Resources.FindObjectsOfTypeAll(Annotation); //typeof(Annotation)); 
        // // foreach(Annotation annot in annot_load) { 
        // //     if (ConfirmButton.selected_types.Contains(annot.type)) { // Modified here 
        // //         annotation_array.Add(annot); 
        // //     } 
        // // } 

        // /* 
        // foreach(Annotation annot in initial_annotations) { 
        //     if(ConfirmButton.selected_types.Contains(annot.type)) { 
        //         annotation_array.Add(annot); 
        //     } 
        // } 
        // */ 


        // // /* 
        // foreach(string type in ConfirmButton.selected_types) { // Modified here 
        //     Debug.Log("loading"); 
        //     Debug.Log(type); 

        //     if(type == "All") { 
        //         continue; 
        //     } 

        //     // Annotation[] annot_load = Resources.LoadAll<Annotation>($"{type}_Annotations"); // 
            
        //     // Annotation[] annot_load = Resources.FindObjectsOfTypeAll<Annotation>(); // 

        //     // Annotation[] annot_load = Resources.FindObjectsOfTypeAll(typeof(Annotation)); 

        //     // var annot_load = Resources.LoadAll($"{type}_Annotations", typeof(Annotation)).Cast<Annotation>(); 


        //     // var annot_load = Resources.LoadAll("", typeof(Annotation)).Cast<Annotation>(); 

        //     // var annot_load = Resources.LoadAll("Restaurants_Annotations", typeof(Annotation)).Cast<Annotation>(); // 

        //     var annot_load = Resources.LoadAll($"{type}_Annotations", typeof(Annotation)).Cast<Annotation>(); // 



        //     // Here!!!! 
        //     // Annotation annot_load = Resources.Load<Annotation>("Restaurants_Annotations/New3.asset"); 
        //     // annotation_array.Add(annot_load); 


        //     // annot = "new.asset"; 
        //     // annotation_array.Add(annot); 

        //     // Debug.Log(annot_load); 
        //     // foreach(Annotation annot in annot_load) { 
        //     //     Debug.Log(annot.name); 
        //     // }
        //     annotation_array.AddRange(annot_load); 
        // } 
    
        // // */ 
        //     // Annotation[] annots = FindAssetsOfType(Annotation); 
        //     // of annots.path includes type 
        //     // Then add it to the actual... 



        // // ''' 
        // // List<string> annotation_guids = new List<string>(); 
        // // // Annotations_array; 
        // // // foreach(string type in selected_types) { // 
        // // foreach(string type in ConfirmButton.selected_types) { // Modified here 
        // //     string[] folder_path = new string[]{$"Assets/Annotations/{type}_Annotations"}; 
        // //     string[] guids = AssetDatabase.FindAssets("t:Annotation", folder_path); 
        // //     annotation_guids.AddRange(guids); 
        // //     // Debug.Log(type); 
        // // } 
        // // foreach(string guid in annotation_guids) { 
        // //     string path = AssetDatabase.GUIDToAssetPath(guid); 
        // //     annotation_array.Add(AssetDatabase.LoadAssetAtPath<Annotation>(path)); 
        // //     // Debug.Log(guid); 
        // // } 
        // // ''' 

        // annotations = annotation_array.ToArray(); // To not redo syntax 

        // Debug.Log("NUMS: "); 
        // Debug.Log($"Last: {annotations.Length}"); 
        // Debug.Log($"Middle: {annotation_array.Count}"); 
        // Debug.Log($"First: {initial_annotations.Length}"); 

        // // */ 

        // // Uncommenting above for manual build 

        
        


// Perhaps?? 
    // Or where those annotations are in the right layer 
    // selected_types = ["Restaurants", "Movies"] // Not Parks 
    // foreach type in selected_types: 
    //     foreach annotation in folder=restaurants: 
    //         annotations.append(annotation); 


        // Debug.Log("Hello"); 
        // foreach (Annotation annot in annotations) { 
        //     Debug.Log(annot); 
        // }
        // Debug.Log(curIndex); 
        // Debug.Log(annotations[curIndex].name); 
        // Debug.Log(annotations[curIndex].description); 
        curIndex += 0; 
        // Debug.Log("Hello"); // 
        // foreach (Annotation annot in annotations) { 
        //     Debug.Log(annot); 
        // } 
        // Debug.Log(curIndex); 
        // Debug.Log(annotations[curIndex].name); 
        // Debug.Log(annotations[curIndex].description); // 

        locationText.text = annotations[curIndex].name;
        descriptionText.text = annotations[curIndex].description;
        artworkImage.sprite = annotations[curIndex].artwork;
        reviewText.text = annotations[curIndex].review;
        addressText.text = annotations[curIndex].address;
    } 



    void Start2() { 
        Debug.Log("NUMS: "); 
        Debug.Log($"Last: {annotations.Length}"); 

        // int idx = 0; 
        // List<string> types = new List<string>(); 
        // foreach (bool flag in ConfirmButton.type_flags) { 
        //     if (flag == true) { 
        //         types.Add(typeMap2[idx]); 
        //     } 
        //     idx++; 
        // } //// 


        // List<string> types = new List<string>(); 
        // if (ConfirmButton.directionState == true) { 
        //     types.Add("Physical"); 
        // } //// 


        // int idx = 0; 
        // List<string> types = new List<string>(); 
        // bool[] flaggers = new bool[]{true, false, false, false}; 
        // foreach (bool flag in flaggers) { 
        //     if (flag == true) { 
        //         types.Add(typeMap2[idx]); 
        //     } 
        //     idx++; 
        // } //// 


        int idx = 0; 
        // bool[] flaggins = new bool[typeMap2.Count]; 
        bool[] flaggins = new bool[]{false, false, false, false}; 
        // if (ConfirmButton.annotationType == true) { 
        //     flaggins[0] = true; 
        // } 
        // } else { 
            // flaggins[2] = true; 
        // } 

        // flaggins[2] = true; 
        // if(ConfirmButton.annotationPhysical == true) { 
        //     flaggins[0] = true; 
        // } /// /// 

        // flaggins[2] = true; 
        // bool oh = false; 
        // if(ConfirmButton.annotationPhysical == true) { 
        // // if(ConfirmButton.annotationState == true) { 
        //     oh = true; 
        // } 
        // if(oh == true) { 
        //     flaggins[0] = true; 
        // } 

        if(ConfirmButton.annotationPhysical == true) { 
            flaggins[0] = true; 
        } 
        if(ConfirmButton.annotationBuilding == true) { 
            flaggins[1] = true; 
        } 
        if(ConfirmButton.annotationVisitor == true) { 
            flaggins[2] = true; 
        } 

        if(ConfirmButton.annotationAll == true) { 
            // flaggins[3] = true; 
            flaggins[0] = true; 
            flaggins[1] = true; 
            flaggins[2] = true; 
        }

        // flaggins[0] = true; 
        // flaggins[2] = true; 
        List<string> types = new List<string>(); 
        foreach (bool flag in flaggins) { 
            if (flag == true) { 
                types.Add(typeMap2[idx]); 
            } 
            idx++; 
        }


        


        // int idx = 0; 
        // List<string> types = new List<string>(); 
        // foreach (bool flag in ConfirmButton.type_flags) { 
        //     if (flag == true) { 
        //         types.Add(typeMap2[idx]); 
        //     } 
        //     idx++; 
        // }




        // foreach(bool flag in ConfirmButton.type_flags) { 
        //     Debug.Log(flag); // print to screen 
        // } 
        // foreach(string type in types) { 
        //     Debug.Log(type); // print to screen 
        // } 


        // List<string> types = new List<string>(); 
        // if (ConfirmButton.directionState == true) { 
        //     types.Add("Physical"); 
        // } 

        // List<string> types = new List<string>(){"Physical"}; //// 


        // // List<string> types = new List<string>(){"Physical"}; // 
        // // List<string> types = new List<string>(); 
        // // foreach (string type in ConfirmButton.selected_types) { 
        // //     types.Add(type); 
        // // } 

        List<string> annotation_names_final = new List<string>(); 
        // foreach (string type in ConfirmButton.selected_types) { 
        foreach (string type in types) { // 
            List<string> sublist = annotation_map[type]; 
            annotation_names_final.AddRange(sublist); 
        } 

        // List<string> annotation_names_final = new List<string>{"Maples Pavilion", "Cantor Arts Center"};  

        // List<string> annotation_names_final = new List<string>{"Maples Pavilion", "Cantor Arts Center"};  

        foreach(Annotation annot in annotations) { 
            // annotation_array.Add(annot); // 
            // if(ConfirmButton.selected_types.Contains(annot.type)) { 
            //     annotation_array.Add(annot); 
            // } 

            // if (annot.name == "Maples Pavilion") {
            //     annotation_array.Add(annot); 
            // } 

            if (annotation_names_final.Contains(annot.name)) { 
                annotation_array.Add(annot); 
            } 

            // if (annot.name == "Maples Pavilion") { 
            //     annotation_array.Add(annot); 
            // } 


            // if (annot.name == "Maples Pavilion") { 
            //     annotation_array.Add(annot); 
            // } 
            // if(ConfirmButton.selected_types.Contains(annot.type)) { 
            //     annotation_array.Add(annot); 
            // } 
        } 

        annotations = annotation_array.ToArray(); 

        Debug.Log("NUMS F: "); 
        Debug.Log($"Last: {annotations.Length}"); // 

        Start(); 




        // Debug.Log("NUMS: "); 
        // Debug.Log($"Last: {annotations.Length}"); 

        // foreach(string type in ConfirmButton.selected_types) { // Modified here 
        //     Debug.Log("loading"); 
        //     Debug.Log(type); 
        //     if(type == "All") { 
        //         continue; 
        //     } 
        //     var annot_load = Resources.LoadAll($"{type}_Annotations", typeof(Annotation)).Cast<Annotation>(); // 
        //     annotation_array.AddRange(annot_load); 
        // } 

        // annotations = annotation_array.ToArray(); 

        // Debug.Log("NUMS F: "); 
        // Debug.Log($"Last: {annotations.Length}"); // 

        // Start(); 


    } 



    public void SetAnnotation(Annotation newAnnotation, bool flip)
    {
        curIndex = System.Array.IndexOf(annotations, newAnnotation); // Fetch index of new annotation... 
        // Debug.Log(curIndex); 
        // Debug.Log(annotations[curIndex].name); 
        // Debug.Log(annotations[curIndex].description); 

        if (ConfirmButton.directionState == false && (flip == true)) { 
            likedAnnotations.enabled = true;
            socialAnnotation.enabled = false;
            return; 
        }


        locationText.text = annotations[curIndex].name;
        descriptionText.text = annotations[curIndex].description;
        artworkImage.sprite = annotations[curIndex].artwork;
        reviewText.text = annotations[curIndex].review;
        addressText.text = annotations[curIndex].address; 
    } 

    void Update() { 

        // Debug.Log($"Conf Rep: {confirmRepopulate}"); 

        if (confirmRepopulate == false) { 
            if (ConfirmButton.confirm == true) { 
                Start2(); 
                // run Start again 
                confirmRepopulate = true; 
            } 
        } 
        // Debug.Log(annotations.Length); 
    }
}
