using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public Texture2D cursorTexture; // Kéo thả hình nền chuột vào đây trong Inspector
    public Vector2 hotspot = new Vector2(0.5f, 0.5f); // Vị trí điểm nóng của con trỏ chuột

    void Awake()
    {
        // Đảm bảo GameObject không bị xóa khi chuyển cảnh
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Thiết lập hình nền chuột
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        // Bạn có thể thêm logic xử lý khác tại đây nếu cần
    }
}
