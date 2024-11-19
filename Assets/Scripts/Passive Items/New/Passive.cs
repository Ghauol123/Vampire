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
        Modifier levelData = (Modifier)data.GetLevelData(++currentLevel);
        currentBoots += levelData.boots;
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
        
        if(level == 1){
            return;
        }
        else{
            Modifier levelData = (Modifier)data.GetLevelData(++level);
            currentBoots += levelData.boots;
        }
        
        if (owner != null && bOTowner == null)
        {
            owner.RecalculatedStats();
        }
        else if(owner != null && bOTowner != null){
            owner.RecalculatedStats();
            bOTowner.RecalculatedStats();
        }
    }
}
