using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombWeapon : ProjectileWeapon
{
    // Override the Attack method to spawn bombs
    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.bombPrefabs)
        {
            Debug.LogWarning(string.Format("bombPrefabs has not been set for {0}", name));
            return false;
        }
        if (!canAttack()) return false;
        // Spawn bomb from the object pool
        GameObject bombObj = ObjectPool.Instance.GetObject(currentStats.bombPrefabs.gameObject);
        // bombObj.transform.position = ownerTransform.transform.position + (Vector3)GetSpawnOffset(GetSpawnAngle());
        bombObj.transform.position = ownerTransform.transform.position;

        bombObj.transform.rotation = Quaternion.identity;

        Bomb bomb = bombObj.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.weapon = this;
            // bomb.owner = owner;
                        if(owner != null && bOTowner == null){
            bomb.owner = owner;
        }
        else{
            bomb.botOwner = bOTowner;
        }
            bomb.Initialize(); // Initialize the bomb
            
            // Ensure the bomb is active and visible
            bombObj.SetActive(true);
            
            // Log for debugging
            Debug.Log("Bomb spawned at: " + bombObj.transform.position);
        }
        else
        {
            Debug.LogError("Bomb component not found on the spawned object!");
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

