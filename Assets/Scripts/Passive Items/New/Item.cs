using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    [HideInInspector]
    public ItemData itemData;
    protected PlayerStats owner;
    public class LevelData{
        public string name;
        public string description;
        public Sprite Icon;

    }
    public virtual void Initialise(ItemData itemData){
        maxLevel = itemData.maxLevel;
        owner = FindAnyObjectByType<PlayerStats>();
    }
    public virtual bool CanLevelUp(){
        return currentLevel <= maxLevel;
    }
    // Whenever an item levels up, attempt to make it evolve
    public virtual bool DoLevelUp(){
        return true;
    }
    //What effects you receive on equipping an item
    public virtual void onEquip(){}
    public virtual void OnUnEquip(){}
}
