using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float currentAttackInterval; // thời gian bắn giữa các viên đạn
    public int currentAttackCount; // giới hạn số viên đạn có thể bắn được trong 1 lần
    Melee currentMelee;
    // private float lastDirectionX;
    protected override void Update()
    {
        base.Update();
        if(currentAttackInterval > 0){
            // nếu > 0 thì trừ cho tới khi = 0 thì tấn công
            currentAttackInterval -= Time.deltaTime;
            if(currentAttackInterval <=0) Attack(currentAttackCount);
        }
        // Kiểm tra tag của parent object
        // GameObject parent = transform.parent.gameObject;
        // if (parent.CompareTag("Player"))
        // {
        //     PlayerMoving playerMove = parent.GetComponent<PlayerMoving>();
        //     if (playerMove != null && playerMove.lastMovedVector.x != 0)
        //     {
        //         lastDirectionX = playerMove.lastMovedVector.x;
        //     }
        // }
        // else if (parent.CompareTag("BOT"))
        // {
        //     BotMoving botMove = parent.GetComponent<BotMoving>();
        //     if (botMove != null && botMove.lastMovedVector.x != 0)
        //     {
        //         lastDirectionX = botMove.lastMovedVector.x;
        //     }
        // }
    }
    public override bool canAttack(){
        if(currentAttackCount >0) return true;
        return base.canAttack();
    }
    // tấn công
    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.meleePrefabs)
        {
            Debug.LogWarning(string.Format("Projectile prefabs has not been set for {0}", name));
            return false;
        }
        if (!canAttack()) return false;

        // // Xác định vị trí spawn dựa trên tag của parent
        // Transform ownerTransform = null;
        // GameObject parent = transform.parent?.gameObject;

        // if (parent != null)
        // {
        //     if (parent.CompareTag("Player"))
        //     {
        //         ownerTransform = owner.transform;
        //     }
        //     else if (parent.CompareTag("BOT"))
        //     {
        //         ownerTransform = bOTowner.transform;
        //     }
        // }

        // if (ownerTransform == null)
        // {
        //     Debug.LogWarning("Invalid parent tag or parent is null");
        //     return false;
        // }
        // Tạo melee object tại vị trí của owner tương ứng
        currentMelee = Instantiate(
            currentStats.meleePrefabs,
            ownerTransform.position,
            Quaternion.identity
        );

        Vector3 scale = currentMelee.transform.localScale;
        scale.x = (lastDirectionX < 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        currentMelee.transform.localScale = scale;

        // Set parent và các thuộc tính khác
        currentMelee.transform.parent = ownerTransform;
        currentMelee.weapon = this;
        // currentMelee.owner = parent.CompareTag("Player") ? owner : bOTowner;
        if(owner != null && bOTowner == null){
            currentMelee.owner = owner;
        }
        else{
            currentMelee.botOwner = bOTowner;
        }
        if (currentCooldown <= 0) currentCooldown += currentStats.cooldown;
        attackCount--;
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }
        return true;
    }
}
