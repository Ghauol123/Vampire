using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : WeaponBehaviour
{
    WeaponController weaponController;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        weaponController = FindObjectOfType<BulletController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += weaponController.speed*direction*Time.deltaTime; // chuyển động và vị trí của viên đạn
    }
}
