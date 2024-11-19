using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    [HideInInspector]
    public ItemData itemData;
    public PlayerStats owner = null;
    public BOTStats bOTowner = null;
    public class LevelData{
        public string name;
        public string description;
        public Sprite Icon;
    }
    protected virtual void Start(){
        AssignOwnerBasedOnParent();
    }
    public virtual void Initialise(ItemData itemData){
        maxLevel = itemData.maxLevel;
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
        public void AssignOwnerBasedOnParent()
{
    // owner = null;
    // bOTowner = null;
    if (transform.parent != null)
    {
        // Check if the parent is a bot
        var botStats = transform.parent.GetComponent<BOTStats>();
        if (botStats != null)
        {
            bOTowner = botStats; // Assign botOwner
            owner = null; // Ensure owner is null
            return;
        }

        // Check if the parent is a player
        var playerStats = transform.parent.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            owner = playerStats; // Assign owner
            bOTowner = null; // Ensure botOwner is null
        }
    }
}
}
