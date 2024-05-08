using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private PlayerMoving player;
    private EnemyStats enemyStats;
    Vector2 knockbackVelocity;
    float knockbackDuration;
    SpriteRenderer _spr;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMoving>();
        _spr = GetComponent<SpriteRenderer>();
        enemyStats = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if(knockbackDuration > 0){
            transform.position += (Vector3)transform.position * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else{
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyStats.currentMoveSpeed * Time.deltaTime);
        }
    }
    public void KnockBack(Vector2 velocity, float duration){
        if(knockbackDuration > 0) return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
