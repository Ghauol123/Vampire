using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player movement and input handling.
/// </summary>
public class PlayerMoving : MonoBehaviour
{
    [HideInInspector] public Vector2 moveDir;
    [HideInInspector] public float lastHorizontalVector;
    [HideInInspector] public float lastVerticalVector;
    [HideInInspector] public Vector2 lastMovedVector;
    [HideInInspector] public Vector2 shootingDirection;

    private Animator _animator;
    private Rigidbody2D rb;
    private PlayerStats playerStats;
    private KeyBindManager keyBindManager;

    // Key bindings
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    // Movement limits
    private const float MaxX = 12f;
    private const float MaxY = 6f;
    private const float MinX = -12f;
    private const float MinY = -6f;

    /// <summary>
    /// Initializes components and references.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = FindObjectOfType<PlayerStats>();
        _animator = GetComponent<Animator>();
        keyBindManager = FindObjectOfType<KeyBindManager>();
        
        lastMovedVector = new Vector2(1, 0f);

        InitializeKeyBindings();
    }

    /// <summary>
    /// Initializes key bindings from PlayerPrefs or defaults.
    /// </summary>
    private void InitializeKeyBindings()
    {
        upKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", KeyCode.W.ToString()));
        downKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", KeyCode.S.ToString()));
        leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", KeyCode.A.ToString()));
        rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", KeyCode.D.ToString()));
    }

    void Update()
    {
        if (IsMovementAllowed())
        {
            InputManagement();
        }
    }

    void FixedUpdate()
    {
        if (IsMovementAllowed())
        {
            Move();
        }
    }

    /// <summary>
    /// Checks if movement is allowed based on game state.
    /// </summary>
    private bool IsMovementAllowed()
    {
        return !(GameManager.instance.IsGameOver || GameManager.instance.IsGamePause || GameManager.instance.IsLevelUp);
    }

    /// <summary>
    /// Manages player input for movement.
    /// </summary>
    private void InputManagement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(upKey)) moveY = 1f;
        else if (Input.GetKey(downKey)) moveY = -1f;

        if (Input.GetKey(leftKey)) moveX = -1f;
        else if (Input.GetKey(rightKey)) moveX = 1f;

        moveDir = new Vector2(moveX, moveY).normalized;
        UpdateLastMovedVector();
    }

    void UpdateLastMovedVector()
    {
        if (moveDir != Vector2.zero)
        {
            lastMovedVector = moveDir;
            shootingDirection = moveDir;
            
            if (moveDir.x != 0)
            {
                lastHorizontalVector = moveDir.x;
            }
            
            if (moveDir.y != 0)
            {
                lastVerticalVector = moveDir.y;
            }
        }
    }

    /// <summary>
    /// Moves the player based on input and constraints.
    /// </summary>
    private void Move()
    {
        Vector2 newPosition = rb.position + moveDir * playerStats.Stats.moveSpeed * Time.fixedDeltaTime;

        // Clamp position to keep player within movement limits
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);

        // Move only if the position has changed
        if (rb.position != newPosition)
        {
            rb.MovePosition(newPosition);
        }
    }
}
