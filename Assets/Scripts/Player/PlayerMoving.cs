using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;
    Animator _animator;
    public const float DEFAULT_MOVESPEED = 5f;
    // References
    Rigidbody2D rb;
    PlayerStats playerStats;

    // Limits for the movement area
    private const float MaxX = 12f;
    private const float MaxY = 6f;
    private const float MinX = -12f; // Assuming a symmetric map, adjust if needed
    private const float MinY = -6f;  // Assuming a symmetric map, adjust if needed

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
        playerStats = FindObjectOfType<PlayerStats>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }
    
    void InputManagement()
    {
        if(GameManager.instance.IsGameOver || GameManager.instance.IsGamePause || GameManager.instance.IsLevelUp){
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir != Vector2.zero)
        {
            if (moveDir.x != 0)
            {
                lastHorizontalVector = moveDir.x;
                lastMovedVector = new Vector2(lastHorizontalVector, 0f);
            }

            if (moveDir.y != 0)
            {
                lastVerticalVector = moveDir.y;
                lastMovedVector = new Vector2(0f, lastVerticalVector);
            }

            if (moveDir.x != 0 && moveDir.y != 0)
            {
                lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
            }
        }
    }
    
    void Move()
    {
        if(GameManager.instance.IsGameOver || GameManager.instance.IsGamePause || GameManager.instance.IsLevelUp){
            return;
        }

        Vector2 newPosition = rb.position + moveDir * DEFAULT_MOVESPEED * playerStats.Stats.moveSpeed * Time.fixedDeltaTime;

        // Check if the new position is within the limits
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);

        rb.MovePosition(newPosition);
    }
}
