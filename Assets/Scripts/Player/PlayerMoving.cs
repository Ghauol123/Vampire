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

    // Key bindings
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

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

        // Initialize key bindings
        upKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", KeyCode.W.ToString()));
        downKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", KeyCode.S.ToString()));
        leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", KeyCode.A.ToString()));
        rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", KeyCode.D.ToString()));
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
        if (GameManager.instance.IsGameOver || GameManager.instance.IsGamePause || GameManager.instance.IsLevelUp)
        {
            return;
        }

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(upKey)) moveY = 1f;
        else if (Input.GetKey(downKey)) moveY = -1f;

        if (Input.GetKey(leftKey)) moveX = -1f;
        else if (Input.GetKey(rightKey)) moveX = 1f;

        moveDir = new Vector2(moveX, moveY).normalized;

        // Update last moved vector
        UpdateLastMovedVector();
    }

    void UpdateLastMovedVector()
    {
        if (moveDir != Vector2.zero)
        {
            lastHorizontalVector = moveDir.x;
            lastVerticalVector = moveDir.y;
            lastMovedVector = moveDir;
        }
    }

    /// <summary>
    /// Moves the player based on input and constraints.
    /// </summary>
    void Move()
    {
        if (GameManager.instance.IsGameOver || GameManager.instance.IsGamePause || GameManager.instance.IsLevelUp || moveDir == Vector2.zero)
        {
            return;
        }

        Vector2 newPosition = rb.position + moveDir * DEFAULT_MOVESPEED * playerStats.Stats.moveSpeed * Time.fixedDeltaTime;

        // Check if the new position is within the limits
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);

        // Only move if position has changed
        if (rb.position != newPosition)
        {
            rb.MovePosition(newPosition);
        }
    }
}
