using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCinematic : MonoBehaviour
{
    void Update()
    {
        // Kiểm tra nếu người chơi nhấn chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            // Chuyển sang scene mới, thay "NextSceneName" bằng tên scene bạn muốn
            SceneManager.LoadScene("Login");
        }
    }
}
