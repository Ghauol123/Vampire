// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerMoving : MonoBehaviour
// {
//     [HideInInspector]
//     public Vector2 moveDir;
//     [SerializeField]
//     private float _speed;
//     [HideInInspector]
//     public float _lastHorizontal;
//     [HideInInspector]
//     public float _lastVertical;
//     private Rigidbody2D _rigibody;
//     public Vector2 _lastMoved;
//     // Start is called before the first frame update
//     void Start()
//     {
//         _rigibody = GetComponent<Rigidbody2D>();
//         _lastMoved = new Vector2(1,0);
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         InputManager();
//     }
//     private void FixedUpdate() {
//         Move();
//     }
//     public void InputManager(){
//         float moveX = Input.GetAxis("Horizontal");
//         float moveY = Input.GetAxis("Vertical");
//         moveDir = new Vector2(moveX,moveY).normalized;
//         if(moveX != 0){
//             _lastHorizontal = moveDir.x;
//             _lastMoved = new Vector2(_lastHorizontal, 0f);
//         }
//         else if(moveY !=0){
//             _lastVertical = moveDir.y;
//             _lastMoved = new Vector2(0f,_lastVertical);
//         }
//         else if(moveX != 0 && moveY!=0){
//             _lastMoved = new Vector2(_lastHorizontal,_lastVertical);
//         }
//     }
//     public void Move(){
//         _rigibody.velocity = new Vector2(moveDir.x * _speed, moveDir.y *_speed);
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    //Movement
    public float moveSpeed;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;

    //References
    Rigidbody2D rb;

    public CharacterScriptableObject characterScriptableObject;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); //If we don't do this and game starts up and don't move, the projectile weapon will have no momentum
    }

    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);    //Last moved X
        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);  //Last moved Y
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);    //While moving
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDir.x * characterScriptableObject.MoveSpeed, moveDir.y * characterScriptableObject.MoveSpeed);
    }
}
