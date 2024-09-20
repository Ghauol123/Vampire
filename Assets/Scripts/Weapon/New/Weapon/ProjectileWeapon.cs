using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public float currentAttackInterval; // thời gian bắn giữa các viên đạn
    public int currentAttackCount; // giới hạn số viên đạn có thể bắn được trong 1 lần
    protected Projectile currentProjectile;
    protected override void Start()
    {
        base.Start();
        currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        currentAttackCount = ((WeaponData)data).baseStats.number;
        Debug.Log(currentAttackInterval);
        Debug.Log(currentAttackCount);
    }
    protected override void Update()
    {
        // currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        base.Update();
        if(currentAttackInterval > 0){
            // nếu > 0 thì trừ cho tới khi = 0 thì tấn công
            currentAttackInterval -= Time.deltaTime;
            if(currentAttackInterval <=0) Attack(currentAttackCount);
        }
        Debug.Log("Current projectile prefab: " + currentStats.projectilePrefabs);
    }
    // có thể tấn công hay không dựa trên số lần tấn công > 0
    public override bool canAttack(){
        if(currentAttackCount >0) return true;
        return base.canAttack();
    }
    // tấn công
    protected override bool Attack(int attackCount = 1)
    {
        // nếu không có prefabs thì không thể tấn công
        if(!currentStats.projectilePrefabs){
            Debug.LogWarning(string.Format("Projectile prefabs has not been set for {0}", name));
            return false;
        }
        if(!canAttack()) return false;
        // 
        float spawnAngle = GetSpawnAngle();
        currentProjectile =Instantiate(
            currentStats.projectilePrefabs,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0,0,spawnAngle)
        );
        Debug.Log(currentStats.projectilePrefabs);
        currentProjectile.weapon = this;
        currentProjectile.owner = owner;
        // currentProjectile.transform.parent = owner.transform;
        if(currentCooldown <=0) currentCooldown += currentStats.cooldown;
        attackCount--;
        if(attackCount >0){
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }
        return true;
    }
    protected virtual float GetSpawnAngle(){
        // trả về góc quay(hướng quay) của nhân vật ở đơn vị radian sau đó chuyển về đơn vị độ
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0){
        // tính toán vị trí xuất phát ngẫu nhiên của vũ khí dựa trên góc quay
        return Quaternion.Euler(0,0,spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
