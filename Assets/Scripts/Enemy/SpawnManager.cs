using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    int currentWaveIndex;
    int currentWaveSpawnCount = 0;
    public WaveData[] waves;
    // public Camera referenceCamera;

    [Tooltip("If there are more than this number of enemies, we will not spawn any more")]
    public uint maximumEnemies = 300;
    public float spawnTimer;
    float currentWaveDuration = 0f;
    public static SpawnManager instance;
    private const float MaxX = 12f;
    private const float MaxY = 6f;
    private const float MinX = -12f; // Assuming a symmetric map, adjust if needed
    private const float MinY = -6f;  // Assuming a symmetric map, adjust if needed

    // Integers to hold current state information
    public int enemiesSpawned;
    public uint enemiesAlive;
    public int timeRemaining;
    public static uint enemyCount = 0;
    public bool boostedByCurse = true;
    private void Start() {
        if(instance) Debug.Log("There are multiple SpawnManagers in the scene");
        instance = this;
    }

    private void Update() {
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;
        maximumEnemies = waves[currentWaveIndex].maxEnemies;
        // Update integer values
        UpdateEnemyCounts();
        UpdateRemainingTime();

        if(spawnTimer <= 0) {
            if(HasWaveEnd()) {
                currentWaveIndex++;
                currentWaveDuration = currentWaveSpawnCount = 0;
                // If we have gone through all the waves, disable this component
                if(currentWaveIndex >= waves.Length) {
                    Debug.Log("All waves have been completed");
                    enabled = false;
                }
                return;
            }
            // Do not spawn enemies if we do not meet the conditions to do so.
            if(!CanSpawn()) {
                // spawnTimer += waves[currentWaveIndex].GetSpawnTime();
                ActiveCooldown();
                return;
            }

            // Get the array of enemies that we are spawning for this tick.
            GameObject[] enemies = waves[currentWaveIndex].GetSpawnPrefabs(EnemyStats.count);
            foreach(GameObject enemy in enemies) {
                if(!CanSpawn()) continue;
                Vector2 spawnPosition = GenerateRandomPosition();
                Instantiate(enemy, spawnPosition, Quaternion.identity);
                currentWaveSpawnCount++;
                enemiesSpawned++; // Update spawned count
                enemiesAlive++; // Update alive count
            }
            // Regenerate the spawn timer
            // spawnTimer += waves[currentWaveIndex].GetSpawnTime();
            ActiveCooldown();
        }
    }

    public void ActiveCooldown() {
        float CurseBoost  = boostedByCurse ? GameManager.GetCumulativeCurse() : 1;
        spawnTimer = waves[currentWaveIndex].GetSpawnTime() / CurseBoost;
    }

    private Vector2 GenerateRandomPosition()
    {
        float randomX = Random.Range(MinX, MaxX);
        float randomY = Random.Range(MinY, MaxY);
        return new Vector2(randomX, randomY);
    }

    bool CanSpawn() {
        // Don't spawn if we have reached the maximum number of enemies
        if(HasExceededMaxEnemies()) return false;
        // Don't spawn if we have reached the maximum number of enemies for this wave
        if(instance.currentWaveSpawnCount > instance.waves[currentWaveIndex].maxEnemies) return false;
        
        // Don't spawn if we have exceeded the wave's duration
        if(instance.currentWaveDuration > instance.waves[currentWaveIndex].spawnDuration) return false;
        return true;
    }

    public static bool HasExceededMaxEnemies() {
        if(!instance) return false;
        if(EnemyStats.count >= instance.maximumEnemies) return true;
        return false;
    }

bool HasWaveEnd() {
    WaveData currentWave = waves[currentWaveIndex];
    bool waveDurationCondition = (currentWave.exitCondition & WaveData.ExitCondition.waveDuration) > 0;
    bool totalSpawnsCondition = (currentWave.exitCondition & WaveData.ExitCondition.reachedTotalSpawns) > 0;

    if (waveDurationCondition && currentWaveDuration >= currentWave.spawnDuration) {
        return true;
    }

    if (totalSpawnsCondition && currentWaveSpawnCount >= currentWave.maxEnemies) {
        return true;
    }

    if (currentWave.mustKillAllEnemies && EnemyStats.count > 0) {
        return false;
    }

    return false;
}




    private void UpdateEnemyCounts() {
        // Update the number of enemies alive
        // Assuming EnemyStats.count provides the current number of enemies alive
        enemiesAlive = (uint)EnemyStats.count;
    }

    private void UpdateRemainingTime() {
        // Calculate the remaining time for the current wave
        if (currentWaveIndex < waves.Length) {
            timeRemaining = Mathf.CeilToInt(waves[currentWaveIndex].spawnDuration - currentWaveDuration);
        }
    }
}
