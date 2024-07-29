using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    int currentWaveIndex;
    int currentWaveSpawnCount = 0;
    public WaveData[] waves;
    public Camera referenceCamera;

    [Tooltip("If there are more than this number of enemies, we will not spawn any more")]
    public int maximumEnemies = 300;
    float spawnTimer;
    float currentWaveDuration = 0f;
    public static SpawnManager instance;
    private void Start() {
        if(instance) Debug.Log("There are multiple SpawnManagers in the scene");
        instance = this;
    }
    private void Update() {
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;
        if(spawnTimer <=0){
            if(HasWaveEnd()){
                currentWaveIndex++;
                currentWaveDuration = currentWaveSpawnCount = 0;
                // If we have gone through all the waves, disable this component
                if(currentWaveIndex >= waves.Length){
                    Debug.Log("All waves have been completed");
                    enabled = false;
                }
                return;
            }
            // Do not spawn enemies if we do not meet the conditions to do so.
            if(!CanSpawn()){
                spawnTimer += waves[currentWaveIndex].GetSpawnTime();
                return;
            }

            //Get the array of enemies that we are spawning for this tick.
            GameObject[] enemies = waves[currentWaveIndex].GetSpawnPrefabs(EnemyStats.count);
            foreach(GameObject enemy in enemies){
                if(!CanSpawn()) continue;
                Instantiate(enemy, GeneratePosition(), Quaternion.identity);
                currentWaveSpawnCount++;
            }
            // Regenerate the spawn timer
            spawnTimer += waves[currentWaveIndex].GetSpawnTime();
        }
    }
    bool CanSpawn(){
        //Don't spawn if we have reached the maximum number of enemies
        if(HasExceededMaxEnemies()) return false;
        //Don't spawn if we have reached the maximum number of enemies for this wave
        if(instance.currentWaveSpawnCount > instance.waves[currentWaveIndex].maxEnemies) return false;
        
        //Don't spawn if we have exceeded the wave's duration
        if(instance.currentWaveDuration > instance.waves[currentWaveIndex].spawnDuration) return false;
        return true;
    }
    public static bool HasExceededMaxEnemies(){
        if(!instance) return false;
        if(EnemyStats.count >= instance.maximumEnemies) return true;
        return false;
    }
    public bool HasWaveEnd(){
        WaveData currentWave = waves[currentWaveIndex];
        //if waveDuration is one of the exit conditions, check how long the waves has been running
        // if current wave duration is not greater than wave duration, do not exit yet
        if((currentWave.exitCondition & WaveData.ExitCondition.waveDuration) > 0){
            if(currentWaveDuration < currentWave.spawnDuration) return false;
        }
        //if reachedTotalSpawns is one of the exit conditions, check how many enemies have been spawned
        // if current wave spawn count is not greater than maxEnemies, do not exit yet
        if((currentWave.exitCondition & WaveData.ExitCondition.reachedTotalSpawns) > 0){
            if(currentWaveSpawnCount < currentWave.maxEnemies) return false;
        }

        //otherwise, if kill all is checked, we have to make sure there are no more enimies first
        if(currentWave.mustKillAllEnemies && EnemyStats.count > 0){
            return false;
        }
        return true;
    }
    private void Reset() {
        referenceCamera = Camera.main;
    }
    public static Vector3 GeneratePosition(){
        //if we do not have a reference camera, create one
        if(!instance.referenceCamera) instance.referenceCamera = Camera.main;

        //if the reference camera is not orthographic, log a warning
        if(!instance.referenceCamera.orthographic){
            Debug.LogWarning("The reference camera is not orthographic, the spawn position may not be correct");
        }

        // generate a position outside of camera boundaries using 2 random numbers
        float x = Random.Range(0f,1f), y = Random.Range(0f,1f);

        // then, randomly choose whether we want to round the x or the y value
        switch(Random.Range(0,2)){
            case 0 : default:
                return instance.referenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(x),y));
            case 1:
                return instance.referenceCamera.ViewportToWorldPoint(new Vector3(x,Mathf.Round(y)));
        }
    }

    //Checking if the enemy is within the camera boundaries
    public static bool IsWithinCameraBounds(Transform checkObject){
        //Get the camera to check if we are within boundaries
        Camera c = instance && instance.referenceCamera ? instance.referenceCamera : Camera.main;

        Vector2 viewport = c.WorldToViewportPoint(checkObject.position);

        if(viewport.x < 0f || viewport.x > 1f) return false;
        if(viewport.y < 0f || viewport.y > 1f) return false;
        return true;
    }
}
