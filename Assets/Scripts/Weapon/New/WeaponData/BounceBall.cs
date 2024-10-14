using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBall : Projectile
{
    public int maxBounces = 3; // Number of times the ball can bounce
    private int currentBounces = 0; // Counter for the current number of bounces

    // Drop onto a random target when initialized
    public override void Initialize()
    {
        base.Initialize(); // Initialize basic projectile properties

        // Find a random target similar to your AcquireAutoAimFacing method
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();
        if (targets.Length > 0)
        {
            EnemyStats randomTarget = targets[Random.Range(0, targets.Length)];
            Vector2 dropPosition = randomTarget.transform.position;
            transform.position = dropPosition; // Drop the ball at the enemy's position
        }

        // Apply downward velocity to simulate the ball falling
        rb.velocity = new Vector2(0, -weapon.GetStats().speed);
    }

    // Handle collision and bouncing with OnTriggerEnter2D
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the object hit is an enemy to deal damage
        EnemyStats enemy = collider.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            enemy.TakeDamage(GetDamage(), source);
        }

        // Check if max bounces have been reached
        if (currentBounces < maxBounces)
        {
            // Reflect the direction using the opposite of the current velocity
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            currentBounces++;
        }
        else
        {
            // If max bounces are reached, return to pool
            ReturnToPool();
        }
    }
}
