
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteLavaWeapon : ProjectileWeapon
{
    public override bool DoLevelUp()
    {
        if(!base.DoLevelUp()) return false;
        // If there is an aura attached to this weapon, we update the aura
        if(currentProjectile){
            currentProjectile.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        return true;
    }
    protected override bool Attack(int attackCount = 1)
    {
        // nếu không có prefabs thì không thể tấn công
        if(!currentStats.projectilePrefabs){
            Debug.LogWarning(string.Format("Elite Miko Lava prefabs has not been set for {0}", name));
            return false;
        }
        if(!canAttack()) return false;
            GameObject projectileObj = ObjectPool.Instance.GetObject(currentStats.projectilePrefabs.gameObject);
    projectileObj.transform.position = ownerTransform.transform.position;
    projectileObj.transform.rotation = Quaternion.identity;

    currentProjectile = projectileObj.GetComponent<Projectile>();
    currentProjectile.weapon = this;
    currentProjectile.owner = owner;
    currentProjectile.Initialize();
        if(currentCooldown <=0){
            currentCooldown += currentStats.cooldown;
        }
        attackCount--;
        if(attackCount >0){
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }
        return true;
    }
}
