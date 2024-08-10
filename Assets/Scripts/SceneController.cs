using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private FirebaseAuth auth;

    void Start()
    {
        // Initialize FirebaseAuth instance
        auth = FirebaseAuth.DefaultInstance;
    }

public static SceneController instance;

    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
        DataPersistenceManager.instance.isNewgame = true;
    }
    public void LogoutUser()
{
    if (auth.CurrentUser != null)
    {
        auth.SignOut();
        Debug.Log("User logged out successfully.");
        // Set the isSignedIn variable to false
        FirebaseController firebaseController = FindObjectOfType<FirebaseController>();
        if (firebaseController != null)
        {
            firebaseController.isSignedIn = false;
        }
        SceneChange("Login");
    }
}

}
