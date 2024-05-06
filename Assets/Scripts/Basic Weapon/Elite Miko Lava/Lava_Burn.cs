using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava_Burn : StateMachineBehaviour
{
    public float timer = 0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
                timer += Time.deltaTime;
        if (timer >= 0.8f) {
            animator.SetBool("IsBurn", true); 
        }   
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
