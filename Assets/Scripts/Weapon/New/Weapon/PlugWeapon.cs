using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugWeapon : ProjectileWeapon
{
protected override bool Attack(int attackCount = 1)
{
    // nếu không có prefabs thì không thể tấn công
    if(!currentStats.projectilePrefabs){
        Debug.LogWarning(string.Format("Projectile prefabs has not been set for {0}", name));
        return false;
    }
    if(!canAttack()) return false;

    // Tìm mục tiêu ngẫu nhiên
    Transform target = FindRandomTarget();
    if(target == null) {
        Debug.LogWarning("No enemies found. Skipping attack.");
        return false; // Không có kẻ địch, không thực hiện tấn công
    }

    // Tính toán góc để nhắm vào mục tiêu
    Vector2 directionToTarget = (target.position - ownerTransform.transform.position).normalized;
    float spawnAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
    // Tạo đạn với hướng nhắm vào mục tiêu
    // currentProjectile = Instantiate(
    //     currentStats.projectilePrefabs,
    //     owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
    //     Quaternion.Euler(0, 0, spawnAngle)
    // );

    // currentProjectile.weapon = this;
    // currentProjectile.owner = owner;
    GameObject projectileObj = ObjectPool.Instance.GetObject(currentStats.projectilePrefabs.gameObject);
    projectileObj.transform.position = ownerTransform.transform.position + (Vector3)GetSpawnOffset(spawnAngle);
    projectileObj.transform.rotation = Quaternion.Euler(0, 0, spawnAngle);

    currentProjectile = projectileObj.GetComponent<Projectile>();
    currentProjectile.weapon = this;
    currentProjectile.owner = owner;
    currentProjectile.Initialize();

    if(currentCooldown <=0) currentCooldown += currentStats.cooldown;
    attackCount--;
    if(attackCount >0){
        currentAttackCount = attackCount;
        currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
    }
    return true;
}
    private Transform FindRandomTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return null;

        int randomIndex = Random.Range(0, enemies.Length);
        return enemies[randomIndex].transform;
    }

}
