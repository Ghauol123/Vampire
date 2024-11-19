using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInventory : MonoBehaviour
{
    [System.Serializable]
    public abstract class BaseSlot
    {
        public Item item;
        public abstract void Assign(Item assignItem);
        public abstract void Clear();
        public bool IstEmpty() { return item == null; }
        public Weapon GetWeapon() { return item as Weapon; }
        public Passive GetPassiveItem() { return item as Passive; }
    }

    [System.Serializable]
    public abstract class BaseStatSlot
    {
        public Item item;
        public abstract void Assign(Item assignItem);
        public abstract void Clear();
        public bool IstEmpty() { return item == null; }
        public Stats GetStats() { return item as Stats; }
    }

    // public List<BaseSlot> weaponSlot = new List<BaseSlot>(6);
    // public List<BaseSlot> passiveSlot = new List<BaseSlot>(4);
    // public List<BaseStatSlot> statSlot = new List<BaseStatSlot>(4);
    public bool isInventoryFullAndMaxLevel;
    
    [Header("UI Element")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassive = new List<PassiveData>();
    public List<StatsData> availableStats = new List<StatsData>();

//     public void SetupWeaponBotOwner(Item item)
// {
//     if (item == null) return;

//     // Check if the weapon's parent is a bot
//     if (item.transform.parent != null && item.transform.parent.GetComponent<PlayerStats>() != null)
//     {
//         item.owner = null; // Set owner to null if it's a bot
//     }
//     else if (item.transform.parent != null && item.transform.parent.GetComponent<BOTStats>() != null)
//     {
//         item.bOTowner = item.transform.parent.GetComponent<BOTStats>(); // Set owner to player
//     }
// }

    // ... rest of the common methods from PlayerInventory ...
} 