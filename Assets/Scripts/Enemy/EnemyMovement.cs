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

    public enum OutOffFrameAction {none, respawnAtEdge, despawn}
    public OutOffFrameAction outOffFrameAction = OutOffFrameAction.respawnAtEdge;
    protected bool spawnedOutOfFrame = false;
    private bool isFlipped = false;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        // spawnedOutOfFrame = !SpawnManager.IsWithinCameraBounds(transform);
        rb = GetComponent<Rigidbody2D>();
        playermoving = FindObjectOfType<PlayerMoving>();
        _spr = GetComponent<SpriteRenderer>();
        enemyStats = GetComponent<EnemyStats>();
        player = playermoving.transform;
    }

    void Update()
    {
        LookAtPlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (knockbackDuration > 0)
        {
            rb.velocity = knockbackVelocity;
            knockbackDuration -= Time.fixedDeltaTime;
        }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * enemyStats.Actual.moveSpeed;
            // HandleOutOfFrameAction();
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
    
    // if the enemy falls outside of the frame, handle it
    // protected virtual void HandleOutOfFrameAction(){
    //     // Handle the enemy when it is out of frame
    //     if(!SpawnManager.IsWithinCameraBounds(transform)){
    //         switch(outOffFrameAction){
    //             case OutOffFrameAction.none : default:
    //                 break;
    //             case OutOffFrameAction.respawnAtEdge:
    //             // if the enemy is outside the camera frame, teleport it back to the edge of the frame
    //                 transform.position = SpawnManager.GeneratePosition();
    //                 break;
    //             case OutOffFrameAction.despawn:
    //                 //Don't destroy if it is spawned outside the frame
    //                 if(!spawnedOutOfFrame){
    //                     Destroy(gameObject);
    //                 }
    //                 break;
    //         }
    //     }
    //     else{
    //         spawnedOutOfFrame = false;
    //     }
    // }
}
