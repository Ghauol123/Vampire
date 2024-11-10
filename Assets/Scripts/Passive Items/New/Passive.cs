using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Passive : Item
{
    [HideInInspector]
    public ItemData data;
    [SerializeField] public CharacterData.Stats currentBoots;
    [SerializeField] public Weapon.Stats currentWeaponBoosts; // Thêm biến lưu trữ stats tăng cường vũ khí
    [System.Serializable]
    public class Modifier : LevelData{
        public string type;
        public CharacterData.Stats boots;
        public Weapon.Stats weaponBoosts; // Thêm stats tăng cường cho vũ khí
    }
    // For dynamically create passive, call initialise to set everything up
    public virtual void Initialise(PassiveData data){
        base.Initialise(data);
        this.data = data;
        currentBoots = data.baseStats.boots;
        currentWeaponBoosts = data.baseStats.weaponBoosts;
    }
    public virtual CharacterData.Stats GetBoots(){
        return currentBoots;
    }
    public virtual Weapon.Stats GetWeaponBoosts(){
        return currentWeaponBoosts;
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
        currentWeaponBoosts += levelData.weaponBoosts;
        // Cập nhật lại stats cho tất cả vũ khí đang được trang bị
        if (owner != null)
        {
            owner.RecalculateWeaponStats();
        }
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
            currentWeaponBoosts += levelData.weaponBoosts;
        }
        
        if (owner != null)
        {
            owner.RecalculatedStats();
            owner.RecalculateWeaponStats();
        }

        Debug.Log($"Set level for {data.name} to {currentLevel}. Current boots: {currentBoots.maxHeal}, Weapon boosts: {currentWeaponBoosts.damage}"); 
    }
}
