using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    protected PlayerStats owner;
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
