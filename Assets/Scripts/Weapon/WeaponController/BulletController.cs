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
        StartCoroutine(Shooting());
    }
    IEnumerator Shooting()
    {
        for (int i = 0; i < 3; i++)
        {
            // Bắn đạn tại điểm xuất phát
            GameObject spawnBullet = Instantiate(wst.Prefabs);
            spawnBullet.transform.position = transform.position;
            spawnBullet.GetComponent<BulletBehaviour>().DirectionChecker(playerMoving.lastMovedVector);
            // Đợi một khoảng thời gian giữa các viên đạn
            yield return new WaitForSeconds(0.1f);
        }


    }
}
