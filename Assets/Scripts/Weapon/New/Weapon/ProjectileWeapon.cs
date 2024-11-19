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
        // Debug.Log(currentAttackInterval);
        // Debug.Log(currentAttackCount);
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
    // protected override bool Attack(int attackCount = 1)
    // {
    //     // nếu không có prefabs thì không thể tấn công
    //     if(!currentStats.projectilePrefabs){
    //         Debug.LogWarning(string.Format("Projectile prefabs has not been set for {0}", name));
    //         return false;
    //     }
    //     if(!canAttack()) return false;
    //     // 
    //     float spawnAngle = GetSpawnAngle();
    //     currentProjectile =Instantiate(
    //         currentStats.projectilePrefabs,
    //         owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
    //         Quaternion.Euler(0,0,spawnAngle)
    //     );
    //     Debug.Log(currentStats.projectilePrefabs);
    //     currentProjectile.weapon = this;
    //     currentProjectile.owner = owner;
    //     // currentProjectile.transform.parent = owner.transform;
    //     if(currentCooldown <=0) currentCooldown += currentStats.cooldown;
    //     attackCount--;
    //     if(attackCount >0){
    //         currentAttackCount = attackCount;
    //         currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
    //     }
    //     return true;
    // }
    protected override bool Attack(int attackCount = 1)
{
    if(!currentStats.projectilePrefabs){
        Debug.LogWarning(string.Format("Projectile prefabs has not been set for {0}", name));
        return false;
    }
    if(!canAttack()) return false;

    float spawnAngle = GetSpawnAngle();
    GameObject projectileObj = ObjectPool.Instance.GetObject(currentStats.projectilePrefabs.gameObject);
    projectileObj.transform.position = ownerTransform.transform.position;
    projectileObj.transform.rotation = Quaternion.Euler(0, 0, spawnAngle);
    // projectileObj.transform.position = owner.transform.position;
    // projectileObj.transform.rotation = Quaternion.identity;
    currentProjectile = projectileObj.GetComponent<Projectile>();
    currentProjectile.weapon = this;
    // currentProjectile.owner = owner;
            if(owner != null && bOTowner == null){
            currentProjectile.owner = owner;
        }
        else{
            currentProjectile.botOwner = bOTowner;
        }
    currentProjectile.Initialize();

    if(currentCooldown <=0) currentCooldown += currentStats.cooldown;
    attackCount--;
    if(attackCount >0){
        currentAttackCount = attackCount;
        currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
    }
    return true;
}
protected virtual float GetSpawnAngle(){
    // Determine the direction based on the actual owner
    if (owner != null) {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    } else if (bOTowner != null) {
        BotMoving botMove = bOTowner.GetComponent<BotMoving>();
        if (botMove != null) {
            return Mathf.Atan2(botMove.lastMovedVector.y, botMove.lastMovedVector.x) * Mathf.Rad2Deg;
        }
    }
    return 0f; // Default angle if no owner is found
}
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0){
        // tính toán vị trí xuất phát ngẫu nhiên của vũ khí dựa trên góc quay
        return Quaternion.Euler(0,0,spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}