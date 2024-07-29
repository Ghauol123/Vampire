using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnData : ScriptableObject
{
    [Tooltip("The enemy prefab to spawn")]
    public GameObject[] possibleSpawnPrefaps = new GameObject[1];
    [Tooltip("Time between each spawns, it will be random between min and max")]
    public Vector2 spawnTime = new Vector2(2, 3);
    [Tooltip("The number of enemies to spawn")]
    public Vector2Int spawnPerTic = new Vector2Int(1, 1);

    [Tooltip("How long this will spawn enemies for")]
    [Min(0.1f)] public float spawnDuration = 60;
    //retur an array of prefabs that we should spawn
    // Takes an optional parameter of how many enemies are on the screen
    public virtual GameObject[] GetSpawnPrefabs(int totalEnemies = 0)
    {
        int count = Random.Range(spawnPerTic.x, spawnPerTic.y);
        GameObject[] result = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = possibleSpawnPrefaps[Random.Range(0, possibleSpawnPrefaps.Length)];
        }
        return result;
    }
    //get a random spawn interval between min and max

    public virtual float GetSpawnTime()
    {
        return Random.Range(spawnTime.x, spawnTime.y);
    }
}
