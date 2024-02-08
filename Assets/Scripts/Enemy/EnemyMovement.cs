using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private PlayerMoving player;
    [SerializeField]
    private float moveSpeed;
    SpriteRenderer _spr;
    // Start is called before the first frame update
    void Start()
    {
        player  = FindObjectOfType<PlayerMoving>();
        _spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position,moveSpeed*Time.deltaTime);
        if(transform.position.x > 0){
            _spr.flipX = false;
        }
        else if(transform.position.x < 0){
            _spr.flipX = true;
        }
    }
}
