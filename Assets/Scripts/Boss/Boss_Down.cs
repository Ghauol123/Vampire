using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Down : Boss_Jump {
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        boss.LookAtPlayer();
        Vector2 target = new Vector2(player.position.x, player.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, Speed* Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.ResetTrigger("Down");
        animator.SetTrigger("Run");
    }
}
