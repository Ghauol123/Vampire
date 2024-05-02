using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Jump : StateMachineBehaviour {
    protected Transform player;
    protected Rigidbody2D rb;
    protected float Speed = 1f;
    float attackRange = 3f;
    protected Boss boss;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        boss.LookAtPlayer();
        Vector2 target = new Vector2(player.position.x, player.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, Speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
          float distance = Vector3.Distance(player.position, rb.position);
            if (distance <= attackRange)
            {
                // Kích hoạt trigger để nhảy
                animator.SetTrigger("Jump");
            }
            else
            {
                // Kích hoạt trigger để chạy
                animator.SetTrigger("Run");
            }
    }
    public void OnJumpExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.SetTrigger("Down");
    }
    public void OnDownExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.SetTrigger("Run");
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Run");
        animator.ResetTrigger("Down");
    }
}
