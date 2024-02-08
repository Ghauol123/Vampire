using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCookingController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnSpiderCooking = Instantiate(prefabs);
        spawnSpiderCooking.transform.position = transform.position;
        spawnSpiderCooking.transform.parent = transform;
    }

}
