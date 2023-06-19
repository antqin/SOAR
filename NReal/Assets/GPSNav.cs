// using System.Collections;
// using UnityEngine;
// using UnityEngine.Android;
//
// public class GPSNav : MonoBehaviour
// {
//     public static GPSNav Instance {set; get;}
//     public float latitude;
//     public float longitude;
//     public float targetLatitude = 0.0f;
//     public float targetLongitude = 0.0f;
//     public float distanceThreshold = 10.0f; // Distance in meters
// 
//     private void Start()
//     {
//         Instance = this;
//         DontDestroyOnLoad(gameObject);
//
//         // Request GPS permission from the user
//         if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//         {
//             Permission.RequestUserPermission(Permission.FineLocation);
//         }
//
//         StartCoroutine(StartLocationService());
//     }
//
//     private IEnumerator StartLocationService()
//     {
//         // Wait until GPS permission is granted by the user
//         while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//         {
//             yield return null;
//         }
//
//         // Check if user has location enabled
//         if (!Input.location.isEnabledByUser)
//         {
//             Debug.LogError("User did not enable location");
//         }
//
//
//         // Start GPS location service
//         Input.location.Start();
//
//         // Wait for GPS location to be obtained
//         while (Input.location.status == LocationServiceStatus.Initializing)
//         {
//             yield return new WaitForSeconds(1);
//         }
//
//         // Check if GPS location is available
//         if (Input.location.status == LocationServiceStatus.Failed)
//         {
//             Debug.LogError("Unable to determine device's location");
//             yield break;
//         }
//
//         // Get GPS coordinates
//         latitude = Input.location.lastData.latitude;
//         longitude = Input.location.lastData.longitude;
//
//         // Check if target location has been reached
//         float distanceToTarget = CalculateDistance(latitude, longitude, targetLatitude, targetLongitude);
//         if (distanceToTarget < distanceThreshold)
//         {
//             // Instantiate a Unity object in the Nreal AR scene
//             print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
//
//
//             // Uncomment this line and fill with object:
//             // var arObject = Instantiate(arObjectPrefab);
//         }
//
//         yield break;
//     }
//
//     private float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
//     {
//         // Calculate distance between two GPS coordinates using the Haversine formula
//         float R = 6371.0f; // Earth's radius in km
//         float dLat = Mathf.Deg2Rad * (lat2 - lat1);
//         float dLon = Mathf.Deg2Rad * (lon2 - lon1);
//         float a = Mathf.Sin(dLat/2) * Mathf.Sin(dLat/2) + Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) * Mathf.Sin(dLon/2) * Mathf.Sin(dLon/2);
//         float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1-a));
//         float distance = R * c * 1000.0f; // Convert distance to meters
//         return distance;
//     }
// }
