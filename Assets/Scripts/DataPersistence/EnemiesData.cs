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
//         public WaveDataSave ToWaveDataSave()
// {
//     if (possibleSpawnPrefaps == null || possibleSpawnPrefaps.Length == 0)
//     {
//         Debug.LogError("possibleSpawnPrefaps is not initialized or empty!");
//         return null;
//     }

//     WaveDataSave data = new WaveDataSave
//     {
//         startingEnemyCount = this.startingEnemyCount,
//         maxEnemies = this.maxEnemies,
//         exitCondition = this.exitCondition,
//         mustKillAllEnemies = this.mustKillAllEnemies,
//         spawnCount = this.spawnCount,
//         possibleSpawnPrefabNames = new string[possibleSpawnPrefaps.Length],
//         spawnTime = this.spawnTime,
//         spawnPerTic = this.spawnPerTic,
//         spawnDuration = this.spawnDuration
//     };

//     for (int i = 0; i < possibleSpawnPrefaps.Length; i++)
//     {
//         if (possibleSpawnPrefaps[i] == null)
//         {
//             Debug.LogError($"Prefab at index {i} is null!");
//             continue; // Skip or handle the null prefab appropriately
//         }
//         data.possibleSpawnPrefabNames[i] = possibleSpawnPrefaps[i].name;
//     }

//     return data;
// }

//     public static List<WaveDataSave> enemiesData = new List<WaveDataSave>();
//     public void FromWaveDataSave(WaveDataSave data)
//     {
//         this.startingEnemyCount = data.startingEnemyCount;
//         this.maxEnemies = data.maxEnemies;
//         this.exitCondition = data.exitCondition;
//         this.mustKillAllEnemies = data.mustKillAllEnemies;
//         this.spawnCount = data.spawnCount;
//         this.spawnTime = data.spawnTime;
//         this.spawnPerTic = data.spawnPerTic;
//         this.spawnDuration = data.spawnDuration;

//         this.possibleSpawnPrefaps = new GameObject[data.possibleSpawnPrefabNames.Length];
//         for (int i = 0; i < data.possibleSpawnPrefabNames.Length; i++)
//         {
//             this.possibleSpawnPrefaps[i] = FindPrefabByName(data.possibleSpawnPrefabNames[i]);
//         }
//     }

//     private GameObject FindPrefabByName(string name)
//     {
//         return Resources.Load<GameObject>(name);
//     }
//     public void SaveWaveData()
// {
//     enemiesData.Clear();
//     foreach (WaveData enemy in FindObjectsOfType<WaveData>())
//     {
//         enemiesData.Add(enemy.ToWaveDataSave());
//     }
// }
}
