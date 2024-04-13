using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixSwordBehaviour : MeleeBehaviour
{
    [SerializeField] private Animator anim;
    private PhoenixSwordController phoenixSwordController;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        phoenixSwordController = FindAnyObjectByType<PhoenixSwordController>();
    }
    void Update()
    {
        transform.position = phoenixSwordController.transform.position + direction*Time.deltaTime; // chuyển động và vị trí của viên đạn
    }
}
