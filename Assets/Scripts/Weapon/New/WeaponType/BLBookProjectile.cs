using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLBookProjectile : Projectile
{
public override void Initialize()
    {
        // Ensure the weapon and owner are assigned before proceeding
        if (weapon == null)
        {
            Debug.LogError("Weapon is not assigned!");
            return;
        }

        base.Initialize();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        if (es)
        {
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            es.TakeDamage(GetDamage(), source);

            // Custom behavior for BLBookProjectile - do not decrement pierce
            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        // No pierce decrement or destroy logic here
    }

}
