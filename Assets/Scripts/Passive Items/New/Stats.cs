using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : Item
{
        [HideInInspector]
    public ItemData data;
    [SerializeField] public CharacterData.Stats currentBoots;
    [System.Serializable]
    public class Modifier : LevelData{
        public string type;
        public CharacterData.Stats boots;
    }
    // For dynamically create passive, call initialise to set everything up
    public virtual void Initialise(StatsData data){
        base.Initialise(data);
        this.data = data;
        currentBoots = data.baseStats.boots;
    }
    public virtual CharacterData.Stats GetBoots(){
        return currentBoots;
    }
    public override bool DoLevelUp()
    {
        if(!CanLevelUp()){
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}. max leve of {2} already reached", name,currentLevel, data.maxLevel));
            return false;
        }
        // If the item can level up, increase the stats
        currentBoots += ((Modifier)data.GetLevelData(++currentLevel)).boots;
        return true;
    }
public virtual void SetLevel(int level)
{
    // Ensure the level is within valid bounds
    if (level < 1 || level > data.maxLevel)
    {
        Debug.LogWarning($"Level {level} is out of bounds for {data.name}.");
        return;
    }

    // Set the current level
    currentLevel = level;

    // Reset boots to base stats
    if (level == 1)
    {
        currentBoots = ((Modifier)data.GetLevelData(level)).boots;  // Reset về stats level 1
        return;
    }
    else
    {
        // Tính toán dựa trên cấp độ hiện tại
        currentBoots += ((Modifier)data.GetLevelData(level)).boots;
    }

    // Gọi hàm để cập nhật stats của nhân vật
    owner.ApplyStatsUpgrade();

    Debug.Log($"Set level for {data.name} to {currentLevel}. Current boots: {currentBoots.maxHeal}"); 
}

}
