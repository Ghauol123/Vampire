using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : Item
{
    public PassiveData data;
    [SerializeField] CharacterData.Stats currentBoots;
    [System.Serializable]
    public struct Modifier{
        public string name, description,type;
        public Sprite Icon;
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
        // otherwise, add stats of the next level to our weapon
        currentBoots += data.GetLevelData(++currentLevel).boots;
        return true;
    }
}
