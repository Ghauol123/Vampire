using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player movement and input handling.
/// </summary>
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
    private const float MinX = -12f;
    private const float MinY = -6f;

    KeyBindManager keyBindManager;  // Reference to KeyBindManager

    /// <summary>
    /// Initializes components and references.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
        playerStats = FindObjectOfType<PlayerStats>();
        _animator = GetComponent<Animator>();

        // Get reference to KeyBindManager
        keyBindManager = FindObjectOfType<KeyBindManager>();
    }

    /// <summary>
    /// Handles input every frame.
    /// </summary>
    void Update()
    {
        InputManagement();
    }

    /// <summary>
    /// Applies movement in fixed time steps.
    /// </summary>
    void FixedUpdate()
    {
        Move();
    }
    
    /// <summary>
    /// Manages player input for movement.
    /// </summary>
    void InputManagement()
    {
        if(GameManager.instance.IsGameOver || GameManager.instance.IsGamePause || GameManager.instance.IsLevelUp){
            return;
        }

        // Use KeyCode from KeyBindManager to check input
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", KeyCode.W.ToString()))))
        {
            moveY = 1f;
        }
        else if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", KeyCode.S.ToString()))))
        {
            moveY = -1f;
        }

        if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", KeyCode.A.ToString()))))
        {
            moveX = -1f;
        }
        else if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", KeyCode.D.ToString()))))
        {
            moveX = 1f;
        }

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
    
    /// <summary>
    /// Moves the player based on input and constraints.
    /// </summary>
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
