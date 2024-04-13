using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixSwordController : WeaponController
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
        StartCoroutine(spawnMeele());
    }
    IEnumerator spawnMeele(){
        SetBulletDirection();
        GameObject spawnSpiderCooking = Instantiate(wst.Prefabs);
        spawnSpiderCooking.transform.position = transform.position;
        spawnSpiderCooking.GetComponent<MeleeBehaviour>().DirectionChecker(bulletDirection);
        spawnSpiderCooking.transform.parent = playerMoving.transform;
        yield return new WaitForSeconds(wst.CooldownDuration);
    }
}
