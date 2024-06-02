using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
    Dictionary<EnemyStats, float> affectedTarges = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targeToUnAffect = new List<EnemyStats>();
    // update is call one frames
    void Update(){
         Dictionary<EnemyStats, float> affectedTargesCopy = new Dictionary<EnemyStats, float>();
        // Loop through every target affected by the aura, and reduce he cooldown;
        // of the aura for it. If the cooldown reaches 0, deal dame to it
        foreach(KeyValuePair<EnemyStats,float> pair in affectedTargesCopy){
            affectedTarges[pair.Key] -= Time.deltaTime;
            if(pair.Value <=0){
                if(targeToUnAffect.Contains(pair.Key)){
                    //If the target is marked for removal, remove it.
                    affectedTarges.Remove(pair.Key);
                    targeToUnAffect.Remove(pair.Key);
                }
                else{
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTarges[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent(out EnemyStats enemyStats)){
            //If the target is not yet affected by this aura, add it
            // to our list of affacted targets
            if(!affectedTarges.ContainsKey(enemyStats)){
                //Always starts with an internal of 0, so that it will get
                //damaged in the next Update() tick
                affectedTarges.Add(enemyStats,0);
            }
            else{
                if(targeToUnAffect.Contains(enemyStats)){
                    targeToUnAffect.Remove(enemyStats);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.TryGetComponent(out EnemyStats enemyStats)){
            //Do not directly remove the target upon leaving,
            //because we still have to track their cooldowns.
            if(affectedTarges.ContainsKey(enemyStats)){
                targeToUnAffect.Add(enemyStats);
            }
        }
    }
}
