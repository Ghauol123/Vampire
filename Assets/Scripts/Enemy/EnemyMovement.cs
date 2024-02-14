using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private PlayerMoving player;
    private EnemyStats enemyStats;
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
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyStats.currentMoveSpeed * Time.deltaTime);
        if (player.moveDir.x > 0)
        {
            _spr.flipX = false;
        }
        else if (player.moveDir.x < 0)
        {
            _spr.flipX = true;
        }
    }
}
