using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    [HideInInspector]
    public Vector2 moveDir;
    [SerializeField]
    private float _speed;
    [HideInInspector]
    public float _lastHorizontal;
    [HideInInspector]
    public float _lastVertical;
    private Rigidbody2D _rigibody;
    // Start is called before the first frame update
    void Start()
    {
        _rigibody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
    }
    private void FixedUpdate() {
        Move();
    }
    public void InputManager(){
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        moveDir = new Vector2(moveX,moveY).normalized;
        if(moveX != 0){
            _lastHorizontal = moveX;
        }
        else if(moveY !=0){
            _lastVertical = moveY;
        }
    }
    public void Move(){
        _rigibody.velocity = new Vector2(moveDir.x * _speed, moveDir.y *_speed);
    }
}
