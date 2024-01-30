using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private PlayerMoving _playerMoving;
    private SpriteRenderer _spriteRender;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerMoving = GetComponent<PlayerMoving>();
        _spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        IsMove();
    }
    public void IsMove(){
        if(_playerMoving.moveDir.x != 0 || _playerMoving.moveDir.y !=0){
            _animator.SetBool("IsRun",true);
        }
        else{
            _animator.SetBool("IsRun",false);
        }
        IsChangeDirection();
    }
    public void IsChangeDirection(){
        if(_playerMoving._lastHorizontal <0){
            _spriteRender.flipX = true;
        }
        else{
            _spriteRender.flipX = false;
        }
    }
}
