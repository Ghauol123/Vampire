// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;

// public class NetworkCheck : MonoBehaviour
// {
//     public GameObject panelNoInternet; // Panel to display when there's no internet
//     public static bool isInternetAvailable = true; // Flag to track internet availability

//     private void Start()
//     {
//         // Hide the panel initially
//         panelNoInternet.SetActive(false);
        
//         // Start checking for network status
//         StartCoroutine(CheckNetworkConnection());
//     }

//     // Coroutine to check network connection repeatedly
//     IEnumerator CheckNetworkConnection()
//     {
//         while (true)
//         {
//             // Check if there's an active internet connection
//             if (Application.internetReachability == NetworkReachability.NotReachable)
//             {
//                 // No internet connection, show the panel
//                 panelNoInternet.SetActive(true);
//                 isInternetAvailable = false; // Update the flag
//                 Debug.Log("No internet connection.");
//             }
//             else
//             {
//                 // Internet is available, hide the panel
//                 panelNoInternet.SetActive(false);
//                 isInternetAvailable = true; // Update the flag
//                 Debug.Log("Connected to the internet.");
//             }

//             // Wait for 2 seconds before checking again
//             yield return new WaitForSeconds(2f);
//         }
//     }
//     public void HidePanel(){
//         // Hide the panel when the "Try Again" button is clicked
//         panelNoInternet.SetActive(false);
//     }
// }
