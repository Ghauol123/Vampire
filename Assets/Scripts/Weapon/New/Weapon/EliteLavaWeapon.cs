using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteLavaWeapon : Weapon
{
    protected float currentAttackInterval; // thời gian bắn giữa các viên đạn
    protected int currentAttackCount; // giới hạn số viên đạn có thể bắn được trong 1 lần

    protected Aura currentAura;

    protected override void Update()
    {
        base.Update();
        if(currentAttackInterval > 0){
            // nếu > 0 thì trừ cho tới khi = 0 thì tấn công
            currentAttackInterval -= Time.deltaTime;
            if(currentAttackInterval <=0) Attack(currentAttackCount);
        }
    }
    public override void onEquip()
    {
        // Try to replace the aura the weapon has with a new one
        if (currentStats.auraPrefabs)
        {
            if (currentAura) Destroy(currentAura);
            // Instantiate the aura at the owner's position without parenting it
            currentAura = Instantiate(currentStats.auraPrefabs, owner.transform.position, Quaternion.identity);
            currentAura.weapon = this;
            currentAura.owner = owner;
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
    }
    public override void OnUnEquip()
    {
        if(currentAura) Destroy(currentAura);
    }
    public override bool DoLevelUp()
    {
        if(!base.DoLevelUp()) return false;
        // If there is an aura attached to this weapon, we update the aura
        if(currentAura){
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        return true;
    }
    protected override bool Attack(int attackCount = 1)
    {
        // nếu không có prefabs thì không thể tấn công
        if(!currentStats.auraPrefabs){
            Debug.LogWarning(string.Format("Elite Miko Lava prefabs has not been set for {0}", name));
            return false;
        }
        if(!canAttack()) return false;
        currentAura =Instantiate(
            currentStats.auraPrefabs, owner.transform.position, Quaternion.identity
        );
        currentAura.weapon = this;
        currentAura.owner = owner;

        if(currentCooldown <=0){
            currentCooldown += currentStats.cooldown;
        }
        attackCount--;
        if(attackCount >0){
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }
}
