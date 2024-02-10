using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject est;
    float currentMoveSpeed;
    public float currentHealts;
    float currentDamage;
    private void Awake() {
        currentDamage = est.Damage;
        currentHealts = est.maxhealt;
        currentMoveSpeed = est.MoveSpeed;
    }
    public void TakeDamage(float dmg){
        currentHealts -= dmg;
        if(currentHealts <= 0){
            Kill();
        }
    }
    public void Kill(){
        Destroy(gameObject);
    }
}
