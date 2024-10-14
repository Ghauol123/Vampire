using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBallWeapon : ProjectileWeapon
{
    protected override bool Attack(int attackCount = 1)
    {
        if(!currentStats.projectilePrefabs){
            Debug.LogWarning("Projectile prefabs has not been set for " + name);
            return false;
        }
        if(!canAttack()) return false;

        // Spawn a bounce ball and drop it onto a random target
        GameObject bounceBallObj = ObjectPool.Instance.GetObject(currentStats.projectilePrefabs.gameObject);
        BounceBall bounceBall = bounceBallObj.GetComponent<BounceBall>();

        // Initialize the bounce ball
        bounceBall.weapon = this;
        bounceBall.owner = owner;
        bounceBall.Initialize();

        // Handle cooldown
        if (currentCooldown <= 0) 
            currentCooldown += currentStats.cooldown;

        attackCount--;
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }

        return true;
    }
}

