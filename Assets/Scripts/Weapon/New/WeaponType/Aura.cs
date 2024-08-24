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
            // Trừ thời gian cooldown cho mỗi kẻ địch bị ảnh hưởng bởi aura
            affectedTargets[pair.Key] -= Time.deltaTime;
            
            if (pair.Value <= 0)
            {
                if (targetsToUnAffect.Contains(pair.Key))
                {
                    // Nếu mục tiêu được đánh dấu để loại bỏ, loại bỏ nó khỏi danh sách
                    affectedTargets.Remove(pair.Key);
                    targetsToUnAffect.Remove(pair.Key);
                }
                else
                {
                    // Kiểm tra xem EnemyStats có còn hợp lệ hay không
                    if (pair.Key != null)
                    {
                        Weapon.Stats stats = weapon.GetStats();
                        affectedTargets[pair.Key] = stats.cooldown;
                        pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                    }
                    else
                    {
                        // Nếu không hợp lệ, loại bỏ khỏi danh sách
                        affectedTargets.Remove(pair.Key);
                    }
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
