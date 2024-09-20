using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Gọi khi nhấn vào Button
    public void Quit()
    {
        // Nếu đang ở trong trình biên dịch Unity Editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;  // Dừng chế độ chơi
        #else
            Application.Quit();  // Thoát khỏi trò chơi khi build ra ngoài
        #endif
    }
}
