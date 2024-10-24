using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete("This script is no longer used, please use EnemySpawnManager.cs instead.")]
public class EnemySpawn : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName; //Tên của đợt tấn công
        public List<EnemyGroup> enemyGroups;
        public int waveQuota; // Tổng số lượng kẻ thù sẽ xuất hiện trong đợt tấn công
        public float spawnInterval; //Khoảng thời gian giữa các lần xuất hiện kẻ thù
        public int spawnCount; // Số lượng kẻ thù đã xuất hiện trong đợt tấn công.
    }
    public int currentWaveCount;
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;  //Danh sách tên của các loại kẻ thù trong đợt tấn công
        public int enemyCount; // Danh sách số lượng kẻ thù của mỗi loại sẽ xuất hiện trong đợt tấn công.
        public int spawnCount; // Số lượng kẻ thù đã xuất hiện trong đợt tấn công.
        public GameObject enemyPrefabs;
    }
    public List<Wave> waves; // list of all wave

    Transform player;
    [Header("Spawn Atttributes")]
    public float spawnTime; // đếm thời gian của spawnInterval
    public float waveInterval;  // thời gian giữa các làn sóng
    public int enemysAlive; // số lượng quái vật hiện tại
    public int maxEnemiesAllowed;   // số lượng quái vật có thể max
    public bool maxEnemiesReached = false; // cho biết số lượng enemies đã max hay chưa
    bool isWaveActive = false;
    [Header("Spawning Position")]
    public List<Transform> spawningPosition;

    // Start is called before the first frame update
    void Start()
    {
        CalculateWaveQuota();
        player = FindObjectOfType<PlayerStats>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }
        spawnTime += Time.deltaTime;
        if (spawnTime >= waves[currentWaveCount].spawnInterval)
        {
            spawnTime = 0f;
            SpawnEnemy();
        }
    }
    IEnumerator BeginNextWave()
    {
        isWaveActive = true;
        yield return new WaitForSeconds(waveInterval);
        if (currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }
    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroups in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroups.enemyCount;
        }
        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }
void SpawnEnemy()
{
    if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
    {
        foreach (var enemyGroups in waves[currentWaveCount].enemyGroups)
        {
            if (enemyGroups.spawnCount < enemyGroups.enemyCount)
            {
                // Sử dụng object pool để lấy enemy từ pool
                GameObject enemy = ObjectPool.Instance.GetObject(enemyGroups.enemyPrefabs);
                
                if (enemy != null)
                {
                    enemy.transform.position = player.position + spawningPosition[Random.Range(0, spawningPosition.Count)].position;
                    enemy.transform.rotation = Quaternion.identity;
                    enemy.SetActive(true);  // Kích hoạt lại đối tượng lấy từ object pool

                    enemyGroups.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemysAlive++;
                    
                    // Kiểm tra nếu đã đạt số lượng kẻ thù tối đa
                    if (enemysAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }
}
    public void OnEnemyKill()
    {
        enemysAlive--;
        if (enemysAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
