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

    // References
    Rigidbody2D rb;
    PlayerStats playerStats;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f);
        playerStats = FindObjectOfType<PlayerStats>();
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
        if(GameManager.instance.IsGameOver){
            return;
        }
        else if(GameManager.instance.IsGamePause){
            return;
        }
        else if(GameManager.instance.isLevelUp){
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
        //     Vector2 movement = new Vector2(moveDir.x * playerStats.currentSpeed, moveDir.y * playerStats.currentSpeed);
        //     rb.MovePosition(rb.position + movement * Time.fixedDeltaTime)
        // rb.velocity = new Vector2(moveDir.x * playerStats.currentSpeed, moveDir.y * playerStats.currentSpeed);
        // Vector2 newPos = Vector2.Lerp(transform.parent.position, moveDir, playerStats.currentSpeed);
        // transform.parent.position = newPos;
        if(GameManager.instance.IsGameOver){
            return;
        }
        else if(GameManager.instance.IsGamePause){
            return;
        }
        else if(GameManager.instance.isLevelUp){
            return;
        }
        Vector2 movement = moveDir * playerStats.CurrentSpeed * Time.fixedDeltaTime;
        transform.Translate(movement);
    }
}
