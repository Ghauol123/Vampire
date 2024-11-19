using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BOTInventory : BaseInventory
{
    [System.Serializable]
    public class Slot : BaseSlot
    {
        public Sprite itemSprite;

        public override void Assign(Item assignItem)
        {
            item = assignItem;
                        if(item is Weapon)
            {
                Weapon w = item as Weapon;
                itemSprite = w.data.icon;
            }
            else if(item is Passive){
                Passive p = item as Passive;
                itemSprite = p.data.icon;
            }
            Debug.Log("Assign " + item.name + " to bot");
        }

        public override void Clear()
        {
            item = null;
        }
    }

    [System.Serializable]
    public class StatSlot : BaseSlot
    {

        public override void Assign(Item assignItem)
        {
            item = assignItem;
            Debug.Log("Assign " + item.name + " to bot");
        }

        public override void Clear()
        {
            item = null;
        }
    }

    public List<Slot> weaponSlot = new List<Slot>(6);
    public List<Slot> passiveSlot = new List<Slot>(4);
    public List<StatSlot> statSlot = new List<StatSlot>(4);
    BOTStats botStats;

    protected virtual void Awake()
    {
        botStats = FindAnyObjectByType<BOTStats>();
    }

    public bool Has(ItemData type) { return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        else if (type is StatsData) return Get(type as StatsData);
        return null;
    }

    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlot)
        {
            Passive p = s.item as Passive;
            if (p && p.data == type)
            {
                return p;
            }
        }
        return null;
    }

    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlot)
        {
            Weapon w = s.item as Weapon;
            if (w && w.data == type)
            {
                return w;
            }
        }
        return null;
    }

    public Stats Get(StatsData type)
    {
        foreach (StatSlot s in statSlot)
        {
            Stats w = s.item as Stats;
            if (w && w.data == type)
            {
                return w;
            }
        }
        return null;
    }

    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is PassiveData) return Add(data as PassiveData);
        else if (data is StatsData) return Add(data as StatsData);
        return -1;
    }

    public int Add(WeaponData data)
    {
        int slotNum = FindEmptySlot(weaponSlot);
        if (slotNum < 0) return slotNum;

        Type weaponType = Type.GetType(data.behaviour);
        if (weaponType != null)
        {
            GameObject go = new GameObject(data.baseStats.name + "Controller");
            Weapon spawnWeapon = (Weapon)go.AddComponent(weaponType);
            spawnWeapon.Initialise(data);
            spawnWeapon.transform.SetParent(transform);
            spawnWeapon.transform.localPosition = Vector2.zero;
            spawnWeapon.onEquip();
            weaponSlot[slotNum].Assign(spawnWeapon);
        }
        else
        {
            Debug.LogWarning($"Invalid weapon type specified for {data.name}");
        }
        return slotNum;
    }

    public int Add(PassiveData data)
    {
        int slotNum = FindEmptySlot(passiveSlot);
        if (slotNum < 0) return slotNum;

        GameObject go = new GameObject(data.baseStats.name + "Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform);
        p.transform.localPosition = Vector2.zero;
        passiveSlot[slotNum].Assign(p);
        botStats.RecalculatedStats();
                if (botStats == null)
        {
            Debug.LogError("botStats is null before applying stats upgrade.");
        }
        else
        {
            botStats.RecalculatedStats();
        }
        return slotNum;
    }

    public int Add(StatsData data)
    {
        int slotNum = FindEmptySlot(statSlot);
        if (slotNum < 0) return slotNum;

        GameObject go = new GameObject(data.baseStats.name + "Stats");
        Stats s = go.AddComponent<Stats>();
        s.Initialise(data);
        s.transform.SetParent(transform);
        s.transform.localPosition = Vector2.zero;
        statSlot[slotNum].Assign(s);

        if (botStats == null)
        {
            Debug.LogError("botStats is null before applying stats upgrade.");
        }
        else
        {
            botStats.ApplyStatsUpgrade();
        }

        return slotNum;
    }

    public bool LevelUp(ItemData data)
    {
        Item item = Get(data);
        if (item) return LevelUp(item);
        return false;
    }

    public bool LevelUp(Item item)
    {
        if (!item.DoLevelUp())
        {
            Debug.LogWarning("Failed to level up " + item.name);
            return false;
        }
        if (item is Passive)
        {
            botStats.RecalculatedStats();
        }
        else if (item is Stats)
        {
            botStats.ApplyStatsUpgrade();
        }
        return true;
    }

    private int FindEmptySlot<T>(List<T> slots) where T : BaseSlot
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IstEmpty())
            {
                return i;
            }
        }
        return -1;
    }

    public void AutoSelectUpgrade()
    {
        List<ItemData> availableUpgrades = new List<ItemData>();
        List<ItemData> allUpgrades = new List<ItemData>(availableWeapons);
        allUpgrades.AddRange(availablePassive);
        allUpgrades.AddRange(availableStats);

        foreach (ItemData data in allUpgrades)
        {
            Item obj = Get(data);

            if (obj)
            {
                if (obj.currentLevel < data.maxLevel)
                {
                    availableUpgrades.Add(data);
                }
            }
            else
            {
                if (data is WeaponData && !Has(data as WeaponData))
                {
                    availableUpgrades.Add(data);
                }
                else if (data is PassiveData && !Has(data as PassiveData))
                {
                    availableUpgrades.Add(data);
                }
                else if (data is StatsData && !Has(data as StatsData))
                {
                    availableUpgrades.Add(data);
                }
            }
        }

        if (availableUpgrades.Count > 0)
        {
            ItemData chosenUpgrade = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];
            Item existingItem = Get(chosenUpgrade);

            if (existingItem != null)
            {
                LevelUp(existingItem);
            }
            else
            {
                if (chosenUpgrade is WeaponData)
                {
                    Add(chosenUpgrade as WeaponData);
                }
                else if (chosenUpgrade is PassiveData)
                {
                    Add(chosenUpgrade as PassiveData);
                }
                else if (chosenUpgrade is StatsData)
                {
                    Add(chosenUpgrade as StatsData);
                }
            }
        }
    }
}
