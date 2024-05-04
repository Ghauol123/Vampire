using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour {
    protected Rigidbody2D rb;
     public float timer = 0f;
 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        rb = animator.GetComponent<Rigidbody2D>();
        Vector2 finalPlayerPosition = animator.GetBehaviour<Boss_Down>().GetFinalPlayerPosition();
        rb.position = finalPlayerPosition;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {

        timer += Time.deltaTime;
        if (timer >= 1.7f) {
            animator.SetTrigger("Run");
            timer = 0f;
        }
    }
}
