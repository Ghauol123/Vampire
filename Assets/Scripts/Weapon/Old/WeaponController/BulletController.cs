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
    private Vector3 bulletDirection;

    // Hàm để thiết lập hướng viên đạn (có thể đặt trong hàm Start hoặc Awake)
    private void SetBulletDirection()
    {
        // Lấy hướng từ playerMoving.lastMovedVector hoặc từ bất kỳ nguồn thông tin hướng nào khác
        bulletDirection = playerMoving.lastMovedVector.normalized;
    }


    protected override void Attack()
    {
        base.Attack();

        Debug.Log("Attack() is called."); // Thêm dòng này để kiểm tra xem Attack() được gọi hay không
        StartCoroutine(Shooting());
    }
    IEnumerator Shooting()
    {
        SetBulletDirection();

        for (int i = 0; i < wst.NumofBullet; i++)
        {
            // Bắn đạn tại điểm xuất phát
            GameObject spawnBullet = Instantiate(wst.Prefabs);
            spawnBullet.transform.position = transform.position;
            spawnBullet.GetComponent<BulletBehaviour>().DirectionChecker(bulletDirection);
            spawnBullet.transform.parent = transform;
            // Đợi một khoảng thời gian giữa các viên đạn
            yield return new WaitForSeconds(0.05f);
        }
    }
}
