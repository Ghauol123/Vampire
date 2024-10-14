using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    private FirebaseAuth auth;
    public static SceneController instance;

    void Start()
    {
        // Khởi tạo FirebaseAuth
        auth = FirebaseAuth.DefaultInstance;
    }

    public void SceneChange(string sceneName)
    {
        // // Ghi lại cảnh hiện tại trước khi chuyển đổi
        // string previousScene = SceneManager.GetActiveScene().name;
        // // Tạo một hành động để theo dõi việc thay đổi cảnh
        // ChangeSceneAction sceneAction = new ChangeSceneAction(previousScene, sceneName);
        // ActionManager.Instance.PerformAction(sceneAction);
        // Tải cảnh mới
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
    public void LogoutUser()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            Debug.Log("Đăng xuất thành công.");
            // Đặt biến isSignedIn thành false
            FirebaseController firebaseController = FindObjectOfType<FirebaseController>();
            if (firebaseController != null)
            {
                firebaseController.isSignedIn = false;
            }
            SceneChange("Login");
        }
    }
}
