using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveDataSave
{
    public float remainingTime; // Thời gian còn lại của wave
    public uint spawnedEnemiesCount; // Số lượng kẻ thù đã xuất hiện
    public int startingEnemyCount; // Số lượng kẻ thù ban đầu
    public uint maxEnemies; // Số lượng kẻ thù tối đa
    public WaveData.ExitCondition exitCondition; // Điều kiện kết thúc wave
    public bool mustKillAllEnemies; // Yêu cầu tiêu diệt hết kẻ thù để kết thúc wave
    public WaveDataSave(){
        remainingTime = 0;
        spawnedEnemiesCount = 0;
        startingEnemyCount = 0;
        maxEnemies = 0;
        exitCondition = 0;
        mustKillAllEnemies = false;
    }
}
