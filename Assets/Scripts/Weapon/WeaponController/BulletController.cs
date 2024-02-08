using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();

        Debug.Log("Attack() is called."); // Thêm dòng này để kiểm tra xem Attack() được gọi hay không
                                          // StartCoroutine(SpawnRoutine());
        GameObject spawnBullet = Instantiate(prefabs);
        spawnBullet.transform.position = transform.position;
        spawnBullet.GetComponent<BulletBehaviour>().DirectionChecker(playerMoving.lastMovedVector);
    }

}
