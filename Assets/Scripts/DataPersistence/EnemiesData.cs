using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class EnemiesData
{
    public Vector3 position;
    public float currentHealth;
    public float currentMoveSpeed;
    public float currentDamage;
    public string enemyPrefabName; // Tên của prefab để có thể load lại enemy


    public EnemiesData(){
        position = Vector3.zero;
        currentHealth = 0;
        currentMoveSpeed = 0;
        currentDamage = 0;
        enemyPrefabName = "";
    }
}
