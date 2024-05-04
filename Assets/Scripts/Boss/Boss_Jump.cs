using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Jump : StateMachineBehaviour {
    protected Transform player;
    protected Rigidbody2D rb;
    protected float Speed = 1f;
    float attackRange = 100f;
    protected Boss boss;
    public float jumpTimer = 0f;
    float jumpInterval = 6f; // Thời gian giữa các lần nhảy

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = FindAnyObjectByType<Boss>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        boss.LookAtPlayer();
        Vector2 target = new Vector2(player.position.x, player.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, Speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        float distance = Vector3.Distance(player.position, rb.position);
        jumpTimer += Time.deltaTime;
        if (distance <= attackRange) {
            if(jumpTimer >= jumpInterval){
                jumpTimer = 0f;
                animator.SetTrigger("Jump");
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.ResetTrigger("Jump");
    }
}
