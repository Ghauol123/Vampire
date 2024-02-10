using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : WeaponBehaviour
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += wst.Speed*direction*Time.deltaTime; // chuyển động và vị trí của viên đạn
    }
}
