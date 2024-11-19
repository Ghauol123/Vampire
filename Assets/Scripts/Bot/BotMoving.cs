using UnityEngine;

public class BotMoving : MonoBehaviour
{
    // Giới hạn di chuyển
    private const float MaxX = 12f;
    private const float MaxY = 6f;
    private const float MinX = -12f;
    private const float MinY = -6f;
    BOTStats bOTStats;

    // Tốc độ di chuyển của bot
    [SerializeField] private float moveSpeed = 5f;
    
    [HideInInspector] public Vector2 lastMovedVector;
    private Vector2 targetPosition;
    private Vector2 currentDirection;
    
    private void Start()
    {
        // Khởi tạo vị trí mục tiêu ngẫu nhiên đầu tiên
        SetNewRandomTarget();
        bOTStats = FindAnyObjectByType<BOTStats>();
        lastMovedVector = Vector2.right; // Khởi tạo hướng mặc định sang phải
    }

    private void Update()
    {
        Vector2 currentPosition = transform.position;
        // Tính toán hướng di chuyển
        currentDirection = (targetPosition - currentPosition).normalized;
        
        // Di chuyển về phía mục tiêu
        transform.position = Vector2.MoveTowards(
            currentPosition, 
            targetPosition, 
            bOTStats.actualStats.moveSpeed * Time.deltaTime
        );

        // Cập nhật lastMovedVector khi bot di chuyển
        if (currentDirection != Vector2.zero)
        {
            lastMovedVector = currentDirection;
        }

        // Nếu đã đến gần mục tiêu, tạo mục tiêu mới
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            SetNewRandomTarget();
        }
    }

    private void SetNewRandomTarget()
    {
        // Tạo vị trí ngẫu nhiên trong phạm vi cho phép
        float randomX = Random.Range(MinX, MaxX);
        float randomY = Random.Range(MinY, MaxY);
        targetPosition = new Vector2(randomX, randomY);
    }
}
