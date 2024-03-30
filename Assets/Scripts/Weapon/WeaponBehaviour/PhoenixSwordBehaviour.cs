using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixSwordBehaviour : MeleeBehaviour
{
    [SerializeField] private Animator anim;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    // private void Update() {
    //     if(destroyAfterSeconds <= 0){
    //         if(Input.GetMouseButtonDown(0)){
    //             anim.SetTrigger("IsAttack");
    //         }
    //         else{
    //             destroyAfterSeconds -= Time.deltaTime;
    //         }
    //     }
    // }
}
