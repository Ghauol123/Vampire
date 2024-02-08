// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CameraMoving : MonoBehaviour
// {
//     [SerializeField]
//     private Transform _player;
//     [SerializeField]
//     private Vector3 offset;
//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         transform.position = _player.transform.position + offset;
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        transform.position = target.position + offset;
    }
}
