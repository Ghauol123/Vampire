using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WaveData", menuName = "ScriptableObject/WaveData", order = 1)]
public class WaveData : SpawnData
{
    [Header("Wave Data")]
    [Tooltip("If there are less than this number of enemies, we will spawn until we get there")]
    [Min(0)] public int startingEnemyCount = 0;
    [Tooltip("How many enemies to spawn maximum")]
    [Min(1)] public uint maxEnemies = uint.MaxValue;
    [System.Flags] public enum ExitCondition { waveDuration =1, reachedTotalSpawns = 2}
    [Tooltip("set the trigger to end this wave")]
    public ExitCondition exitCondition = (ExitCondition)1;
    [Tooltip("All enemies must be dead for the wave to advance")]
    public bool mustKillAllEnemies = false;

    [HideInInspector] public uint spawnCount; // the number of enemies already spawned in this wave

    public override GameObject[] GetSpawnPrefabs(int totalEnemies = 0)
    {
        int count = Random.Range(spawnPerTic.x, spawnPerTic.y);

        //if we have less than <minEnemies> enemies, spawn until we have <minEnemies>

        if (totalEnemies + count < startingEnemyCount)
        {
            count = startingEnemyCount - totalEnemies;
        }

        //generate the result
        GameObject[] result = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = possibleSpawnPrefaps[Random.Range(0, possibleSpawnPrefaps.Length)];
        }
        return result;
    }
}
