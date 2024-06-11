using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private PlayerMoving playermoving;
    private Transform player;
    private EnemyStats enemyStats;
    Vector2 knockbackVelocity;
    float knockbackDuration;
    SpriteRenderer _spr;
    private bool isFlipped = false;
    protected Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playermoving = FindObjectOfType<PlayerMoving>();
        player = FindObjectOfType<PlayerStats>().transform;
        _spr = GetComponent<SpriteRenderer>();
        enemyStats = GetComponent<EnemyStats>();
    }

    void Update()
    {
        LookAtPlayer();
    }

    void FixedUpdate()
    {
        if (knockbackDuration > 0)
        {
            rb.velocity = knockbackVelocity;
            knockbackDuration -= Time.fixedDeltaTime;
        }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * enemyStats.currentMoveSpeed;
        }
    }

    public void KnockBack(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

    public void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float dotProduct = Vector3.Dot(directionToPlayer, transform.right);

        if (dotProduct > 0 && isFlipped)
        {
            Flip();
        }
        else if (dotProduct < 0 && !isFlipped)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFlipped = !isFlipped;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
