using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    private FirebaseAuth auth;
    public static SceneController instance;
    GameMode gamemode;
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
    public void MapChoose(){
        // CharacterSelected.instance.nameMap = sceneName;
            PlayerPrefs.SetInt("GameMode", (int)gamemode);  // Lưu gamemode vào PlayerPrefs
    SceneManager.LoadScene(CharacterSelected.instance.nameMap);
        // SceneManager.LoadScene(CharacterSelected.instance.nameMap);
    }
    public void LogoutUser()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            Debug.Log("Đăng xuất thành công.");
            // Đặt biến isSignedIn thành false
            if (FirebaseController.instance != null)
            {
                FirebaseController.instance.isSignedIn = false;
            }
            SceneChange("Login");
        }
    }
}
