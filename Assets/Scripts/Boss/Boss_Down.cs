using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Down : StateMachineBehaviour {
    public float timer = 0f;
    protected Transform player;
    protected Rigidbody2D rb;
    protected Vector2 finalPlayerPosition;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        finalPlayerPosition = player.position;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        // Tăng timer
        timer += Time.deltaTime;
        // Nếu timer đạt 20 giây, kích hoạt trigger "Down" và reset timer
        if (timer >= 3f) {
            animator.SetTrigger("Down");
            timer = 0f;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.ResetTrigger("Down");
        timer = 0f; // Đảm bảo reset timer khi thoát state này
    }
    public Vector2 GetFinalPlayerPosition() {
        return finalPlayerPosition;
    }
}
