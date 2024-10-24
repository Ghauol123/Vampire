using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnAffect = new List<EnemyStats>();

void Update()
{
    Dictionary<EnemyStats, float> affectedTargetsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

    foreach (KeyValuePair<EnemyStats, float> pair in affectedTargetsCopy)
    {
        // Check if the EnemyStats object is null before any operation
        if (pair.Key == null)
        {
            // If it has been destroyed, remove it from the list
            affectedTargets.Remove(pair.Key);
            continue;
        }

        // Subtract cooldown time for each enemy affected by the aura
        affectedTargets[pair.Key] -= Time.deltaTime;
        
        // Check if cooldown time <= 0
        if (affectedTargets[pair.Key] <= 0)
        {
            if (targetsToUnAffect.Contains(pair.Key))
            {
                // If the target is marked for removal, remove it from the list
                affectedTargets.Remove(pair.Key);
                targetsToUnAffect.Remove(pair.Key);
            }
            else
            {
                // If still valid, reset cooldown and deal damage
                Weapon.Stats stats = weapon.GetStats();
                affectedTargets[pair.Key] = stats.cooldown;
                pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
            }
        }
    }
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats enemyStats))
        {
            if (!affectedTargets.ContainsKey(enemyStats))
            {
                affectedTargets.Add(enemyStats, 0);
            }
            else
            {
                if (targetsToUnAffect.Contains(enemyStats))
                {
                    targetsToUnAffect.Remove(enemyStats);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats enemyStats))
        {
            if (affectedTargets.ContainsKey(enemyStats))
            {
                targetsToUnAffect.Add(enemyStats);
            }
        }
    }
}
