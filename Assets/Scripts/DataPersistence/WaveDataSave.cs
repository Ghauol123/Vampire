using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveDataSave
{
    public int startingEnemyCount;
    public uint maxEnemies;
    public WaveData.ExitCondition exitCondition;
    public bool mustKillAllEnemies;
    public uint spawnCount;
    public string possibleSpawnPrefabNames;
    public Vector2 spawnTime;
    public Vector2Int spawnPerTic;
    public float spawnDuration;
    public int timeRemaining;
    public int enemiesSpawned;
    public WaveDataSave()
    {
        startingEnemyCount = 0;
        maxEnemies = 0;
        exitCondition = 0;
        mustKillAllEnemies = false;
        spawnCount = 0;
        possibleSpawnPrefabNames = "";
        spawnTime = Vector2.zero;
        spawnPerTic = Vector2Int.zero;
        spawnDuration = 0;
        timeRemaining = 0;
        enemiesSpawned = 0;
    }
}
