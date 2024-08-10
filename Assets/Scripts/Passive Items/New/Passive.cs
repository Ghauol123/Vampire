using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Passive : Item
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
    public virtual void Initialise(PassiveData data){
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
    currentBoots = ((PassiveData)data).baseStats.boots;
    // Apply cumulative stats for each level up to the current level
    // for (int i = 1; i < currentLevel; i++)
    // {
    //     if(currentLevel == 1){
    //         return;
    //     }
    //     else{
    //         currentBoots += ((Modifier)data.GetLevelData(++i)).boots;
    //     }
    // }
    if(level == 1){
        return;
    }
    else{
        currentBoots += ((Modifier)data.GetLevelData(++level)).boots;
    }
    // currentBoots += ((Modifier)data.GetLevelData(++currentLevel)).boots;
    owner.RecalculatedStats();

    Debug.Log($"Set level for {data.name} to {currentLevel}. Current boots: {currentBoots.maxHeal}"); 
}

}
