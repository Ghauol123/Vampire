using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteLavaController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Attack()
    {
        base.Attack();
        
        // Tạo ra một điểm ngẫu nhiên trong bán kính 2 đơn vị xung quanh vị trí của người chơi
        Vector2 randomOffset = Random.insideUnitCircle * 2f;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
        
        // Sinh ra đối tượng tại vị trí ngẫu nhiên
        GameObject spawnSpiderCooking = Instantiate(wst.Prefabs);
        spawnSpiderCooking.transform.position = spawnPosition;
    }
}
