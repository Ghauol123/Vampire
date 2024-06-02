using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float currentAttackInterval; // thời gian bắn giữa các viên đạn
    public int currentAttackCount; // giới hạn số viên đạn có thể bắn được trong 1 lần
    Melee currentMelee;
    private float lastDirectionX;
    protected override void Update()
    {
        base.Update();
        if(currentAttackInterval > 0){
            // nếu > 0 thì trừ cho tới khi = 0 thì tấn công
            currentAttackInterval -= Time.deltaTime;
            if(currentAttackInterval <=0) Attack(currentAttackCount);
        }
        // Cập nhật hướng di chuyển cuối cùng trên trục x nếu có sự thay đổi
        if (movement.lastMovedVector.x != 0)
        {
            lastDirectionX = movement.lastMovedVector.x;
        }
    }
    public override bool canAttack(){
        if(currentAttackCount >0) return true;
        return base.canAttack();
    }
    // tấn công
    protected override bool Attack(int attackCount = 1)
    {
        // nếu không có prefabs thì không thể tấn công
        if(!currentStats.meleePrefabs){
            Debug.LogWarning(string.Format("Projectile prefabs has not been set for {0}", name));
            return false;
        }
        if(!canAttack()) return false;
        // Instantiate the melee object at the new position
        currentMelee = Instantiate(
            currentStats.meleePrefabs,
            owner.transform.position,
            Quaternion.identity
        );
        Vector3 scale = currentMelee.transform.localScale;
        scale.x = (lastDirectionX < 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        currentMelee.transform.localScale = scale;


        currentMelee.transform.parent = owner.transform;
        currentMelee.weapon = this;
        currentMelee.owner = owner;

        if(currentCooldown <=0) currentCooldown += currentStats.cooldown;
        attackCount--;
        if(attackCount >0){
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }
}
