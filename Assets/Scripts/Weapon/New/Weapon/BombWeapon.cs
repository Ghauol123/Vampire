using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWeapon : ProjectileWeapon
{
    protected override void Update(){
        base.Update();
        Debug.Log("Current bomb prefab: " + currentStats.bombPrefabs);
    }
    public override bool canAttack(){
        return base.canAttack();
    }
     protected override bool Attack(int attackCount = 1)
    {
        // nếu không có prefabs thì không thể tấn công
        if(!currentStats.bombPrefabs){
            Debug.LogWarning(string.Format("bombPrefabs has not been set for {0}", name));
            return false;
        }
        if(!canAttack()) return false;
        // 
        Bomb currentProjectile =Instantiate(
            currentStats.bombPrefabs,
            transform.position,
            Quaternion.identity
        );
        Debug.Log(currentStats.bombPrefabs);
        currentProjectile.weapon = this;
        currentProjectile.owner = owner;

        if(currentCooldown <=0) currentCooldown += currentStats.cooldown;
        attackCount--;
        if(attackCount >0){
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }
        return true;
    }

}
