using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Passive : Item
{
    [HideInInspector]
    public ItemData data;
    [SerializeField] CharacterData.Stats currentBoots;
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
}
