using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychoAxeController : WeaponController
{
    public Rigidbody2D theRB;
    public float rotaSpeed;
    // Start is called before the first frame update
    protected override void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnSpiderCooking = Instantiate(wst.Prefabs);
        // spawnSpiderCooking.transform.position = transform.position;
        // spawnSpiderCooking.transform.parent = transform;
                theRB.velocity = new Vector2(UnityEngine.Random.Range(-wst.Speed, wst.Speed), wst.Speed);
        transform.rotation = Quaternion.Euler(0f,0f,transform.rotation.eulerAngles.z + (rotaSpeed + 360f * Time.deltaTime*Math.Sign(theRB.velocity.x)));
    }
}
