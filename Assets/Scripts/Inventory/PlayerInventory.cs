using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using JetBrains.Annotations;
using System.Linq;

/// <summary>
/// Manages the player's inventory, including weapons, passive items, and stats.
/// This class handles adding, removing, and upgrading items, as well as
/// managing the UI for item upgrades. It interacts with the player's stats
/// and the game manager to apply changes and handle level-up scenarios.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    /// <summary>
    /// Represents a slot in the inventory for weapons and passive items.
    /// Handles assigning, clearing, and retrieving items from the slot.
    /// </summary>
    [System.Serializable]
    public class Slot{
        public Item item;
        public UnityEngine.UI.Image image;

        /// <summary>
        /// Assigns an item to the slot and updates the UI accordingly.
        /// </summary>
        /// <param name="assignItem">The item to be assigned to the slot.</param>
        public void Assign(Item assignItem){
            item = assignItem;
            if(item is Weapon){
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else{
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log("Assign " + item.name + " to player");
        }

        /// <summary>
        /// Clears the slot, removing the item and resetting the UI.
        /// </summary>
        public void Clear(){
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        /// <summary>
        /// Checks if the slot is empty.
        /// </summary>
        /// <returns>True if the slot is empty, false otherwise.</returns>
        public bool IstEmpty() { return item == null;}

        /// <summary>
        /// Retrieves the weapon from the slot, if present.
        /// </summary>
        /// <returns>The weapon in the slot, or null if not a weapon.</returns>
        public Weapon GetWeapon()
        {
            return item as Weapon;
        }

        /// <summary>
        /// Retrieves the passive item from the slot, if present.
        /// </summary>
        /// <returns>The passive item in the slot, or null if not a passive item.</returns>
        public Passive GetPassiveItem()
        {
            return item as Passive;
        }
    }
    
    /// <summary>
    /// Represents a slot in the inventory specifically for stat items.
    /// Handles assigning, clearing, and retrieving stat items from the slot.
    /// </summary>
    [System.Serializable]
    public class StatSlot{
        public Item item;

        /// <summary>
        /// Assigns a stat item to the slot.
        /// </summary>
        /// <param name="assignItem">The stat item to be assigned.</param>
        public void Assign(Item assignItem){
            item = assignItem;
            Debug.Log("Assign " + item.name + " to player");
        }

        /// <summary>
        /// Clears the stat slot.
        /// </summary>
        public void Clear(){
            item = null;
        }

        /// <summary>
        /// Checks if the stat slot is empty.
        /// </summary>
        /// <returns>True if the slot is empty, false otherwise.</returns>
        public bool IstEmpty() { return item == null;}

        /// <summary>
        /// Retrieves the stats item from the slot.
        /// </summary>
        /// <returns>The stats item in the slot, or null if not a stats item.</returns>
        public Stats GetStats()
        {
            return item as Stats;
        }
    }
    public List<Slot> weaponSlot = new List<Slot>(6);
    public List<Slot> passiveSlot = new List<Slot>(6);
    public List<StatSlot> statSlot = new List<StatSlot>(4);

    /// <summary>
    /// Represents the UI elements for displaying upgrade options to the player.
    /// Contains references to text displays, icons, and buttons for upgrades.
    /// </summary>
    [System.Serializable]
    public class UpgradeUI{
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescripionDisplay;
        public TMP_Text upgradeTypeDisplay;
        public UnityEngine.UI.Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Element")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassive = new List<PassiveData>();
    public List<StatsData> availableStats = new List<StatsData>();
    public List<UpgradeUI> upgradeUIOpitons = new List<UpgradeUI>();
    PlayerStats playerStats;
    private void Awake() {
    playerStats = FindAnyObjectByType<PlayerStats>();
}
    // private void Start() {
    //     playerStats = FindAnyObjectByType<PlayerStats>();
    // }
    // Check if the inventory has an item of a certain type
    public bool Has(ItemData type) {return Get(type);}
    // Get an item from the inventory based on its type
    public Item Get(ItemData type){
        //Determine what type of item we are looking for
        if(type is WeaponData) return Get(type as WeaponData);
        else if(type is PassiveData) return Get(type as PassiveData);
        else if(type is StatsData) return Get(type as StatsData);
        return null;
    }
    // Find a passive item of a certain type in the inventory
    public Passive Get(PassiveData type){
        //Iterate through all the passive slots
        foreach(Slot s in passiveSlot){
            Passive p = s.item as Passive;
            if(p && p.data == type){
                return p;
            }
        }
        return null;
    }
    // Find a weapon of a certain type in the inventory
    public Weapon Get(WeaponData type){
        //Iterate through all the weapon slots
        foreach(Slot s in weaponSlot){
            Weapon w = s.item as Weapon;
            if(w && w.data == type){
                return w;
            }
        }
        return null;
    }
    public Stats Get(StatsData type){
        //Iterate through all the weapon slots
        foreach(StatSlot s in statSlot){
            Stats w = s.item as Stats;
            if(w && w.data == type){
                return w;
            }
        }
        return null;
    }
    // Remove a weapon of a particular type from the inventory
    public bool Remove(WeaponData data, bool removeUpgradeAvailable = false){
        if(removeUpgradeAvailable) availableWeapons.Remove(data);
        for(int i=0; i< weaponSlot.Count; i++){
            Weapon w = weaponSlot[i].item as Weapon;
            if(w.data == data){
                weaponSlot[i].Clear();
                w.OnUnEquip();
                Destroy(w.gameObject);
                return true;
            }
        }
        return false;
    }
    // Remove a passive item of a particular type from the inventory
    public bool Remove(PassiveData data, bool removeUpgradeAvailable = false){
        if(removeUpgradeAvailable) availablePassive.Remove(data);
        for(int i=0; i< passiveSlot.Count; i++){
            Passive p = passiveSlot[i].item as Passive;
            if(p.data == data){
                passiveSlot[i].Clear();
                p.OnUnEquip();
                Destroy(p.gameObject);
                return true;
            }
        }
        return false;
    }
    public bool Remove(StatsData data, bool removeUpgradeAvailable = false){
        if(removeUpgradeAvailable) availableStats.Remove(data);
        for(int i=0; i< statSlot.Count; i++){
            Stats s = statSlot[i].item as Stats;
            if(s.data == data){
                statSlot[i].Clear();
                s.OnUnEquip();
                Destroy(s.gameObject);
                return true;
            }
        }
        return false;
    }
    // Add a weapon to the inventory
    public int Add(WeaponData data){
        int slotNum = 1;
        //try to find an empty slot
        for(int i = 0; i< weaponSlot.Capacity; i++){
            if(weaponSlot[i].IstEmpty()){
                slotNum = i;
                break;
            }
        }
        //If there is no empty slot, exit;
        if(slotNum <0) return slotNum;
        //Otherwise create the weapon in the slot
        // Get the type of the weapon we want to spawn
        Type weaponType = Type.GetType(data.behaviour);
        if(weaponType != null){
            //Spawn  the weapon Gameobject
            GameObject go = new GameObject(data.baseStats.name + "Controller");
            Weapon spawnWeapon = (Weapon)go.AddComponent(weaponType);
            spawnWeapon.Initialise(data);
            spawnWeapon.transform.SetParent(transform);
            spawnWeapon.transform.localPosition = Vector2.zero;
            spawnWeapon.onEquip();
            weaponSlot[slotNum].Assign(spawnWeapon);
            if(GameManager.instance != null && GameManager.instance.chosingUpgrade){
                GameManager.instance.EndLevelUp();
                return slotNum;
            }
        }
        else{
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}",data.name));
        }
        return -1;
    }
    // Get a weapon from a specific slot in the inventory
    public Weapon GetWeaponBySlot(int slotNum)
    {
        if (slotNum >= 0 && slotNum < weaponSlot.Capacity && !weaponSlot[slotNum].IstEmpty())
        {
            return weaponSlot[slotNum].GetWeapon(); // Assuming GetWeapon retrieves the weapon from the slot
        }
        return null;
    }
    // Get a passive item from a specific slot in the inventory
    public Passive GetPassiveBySlot(int slotNum)
    {
        if (slotNum >= 0 && slotNum < passiveSlot.Capacity && !passiveSlot[slotNum].IstEmpty())
        {
            return passiveSlot[slotNum].GetPassiveItem(); // Assuming GetWeapon retrieves the weapon from the slot
        }
        return null;
    }
    // Get a stats item from a specific slot in the inventory
    public Stats GetStatsBySlot(int slotNum)
    {
        if (slotNum >= 0 && slotNum < statSlot.Capacity && !statSlot[slotNum].IstEmpty())
        {
            return statSlot[slotNum].GetStats(); // Assuming GetWeapon retrieves the weapon from the slot
        }
        return null;
    }
    
    // Add a passive item to the inventory
    public int Add(PassiveData data){
        int slotNum = 1;
        //try to find an empty slot
        for(int i = 0; i< passiveSlot.Capacity; i++){
            if(passiveSlot[i].IstEmpty()){
                slotNum = i;
                break;
            }
        }
        //If there is no empty slot, exit;
        if(slotNum <0) return slotNum;
        //Otherwise create the weapon in the slot
        // Get the type of the weapon we want to spawn
        GameObject go = new GameObject(data.baseStats.name+"Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); //Set the weapon to be a child of the player
        p.transform.localPosition = Vector2.zero;
        //Assign the passive to the slot
        passiveSlot[slotNum].Assign(p);
        if(GameManager.instance != null && GameManager.instance.chosingUpgrade){
            GameManager.instance.EndLevelUp();
        }
        playerStats.RecalculatedStats();
        return slotNum;
    }
    // Add a stats item to the inventory
    public int Add(StatsData data){
        int slotNum = 1;
        //try to find an empty slot
        for(int i = 0; i< statSlot.Capacity; i++){
            if(statSlot[i].IstEmpty()){
                slotNum = i;
                break;
            }
        }
        //If there is no empty slot, exit;
        if(slotNum <0) return slotNum;
        //Otherwise create the weapon in the slot
        // Get the type of the weapon we want to spawn
        GameObject go = new GameObject(data.baseStats.name+"Stats");
        Stats s = go.AddComponent<Stats>();
        s.Initialise(data);
        s.transform.SetParent(transform); //Set the weapon to be a child of the player
        s.transform.localPosition = Vector2.zero;
        //Assign the passive to the slot
        statSlot[slotNum].Assign(s);
        if(GameManager.instance != null && GameManager.instance.chosingUpgrade){
            GameManager.instance.EndLevelUp();
        }
        playerStats.ApplyStatsUpgrade();
        return slotNum;
    }

    
    // Add an item to the inventory based on its type
    public int Add(ItemData data){
        if(data is WeaponData) return Add(data as WeaponData);
        else if(data is PassiveData) return Add(data as PassiveData);
        else if(data is StatsData) return Add(data as StatsData);
        return -1;
    }
    // Level up an item in the inventory
    public bool LevelUp(ItemData data){
        Item item = Get(data);
        if(item) return LevelUp(item);
        return false;
    }
    // Level up a specific item
    public bool LevelUp(Item item){
        if(!item.DoLevelUp()){
            Debug.LogWarning("Failed to level up" + item.name);
            return false;
        }
        if(GameManager.instance != null && GameManager.instance.chosingUpgrade){
            GameManager.instance.EndLevelUp();
        }
        if(item is Passive){
            playerStats.RecalculatedStats();
        }
        else if(item is Stats){
            playerStats.ApplyStatsUpgrade();
        }
        return true;
    }
    // Get the number of empty slots in a list of slots
    int GetSlotLeft(List<Slot> slots){
        int count = 0;
        foreach(Slot s in slots){
            if(s.IstEmpty()) count++;
        }
        return count;
    }
    // Get the number of empty slots in a list of stat slots
    int GetSlotLeft(List<StatSlot> slots){
        int count = 0;
        foreach(StatSlot s in slots){
            if(s.IstEmpty()) count++;
        }
        return count;
    }
    // Clear all items from the inventory
    public void ClearInventory()
{
    foreach (var slot in weaponSlot)
    {
        slot.Clear();
    }
    foreach (var slot in passiveSlot)
    {
        slot.Clear();
    }
    foreach (var slot in statSlot)
    {
        slot.Clear();
    }
}
    // Apply upgrade options to the UI
    void ApplyUpgradeOptions()
{
    if (upgradeUIOpitons == null)
    {
        Debug.LogError("upgradeUIOpitons is null!");
        return;
    }

    // Make a duplication of the available weapon/passive upgrade lists
    List<ItemData> availableUpgrades = new List<ItemData>();
    List<ItemData> allUpgrades = new List<ItemData>(availableWeapons);
    allUpgrades.AddRange(availablePassive);
    allUpgrades.AddRange(availableStats);

    // Iterate through each slot in the upgrade UI
    int weaponSlotList = GetSlotLeft(weaponSlot);
    int passiveSlotList = GetSlotLeft(passiveSlot);
    int statSlotList = GetSlotLeft(statSlot);

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
            // Chỉ thêm vũ khí mới nếu còn slot trống và chưa có trong kho đồ
            if (data is WeaponData && weaponSlotList > 0 && !Has(data as WeaponData))
            {
                availableUpgrades.Add(data);
            }
            else if (data is PassiveData && passiveSlotList > 0 && !Has(data as PassiveData))
            {
                availableUpgrades.Add(data);
            }
            else if (data is StatsData && statSlotList > 0 && !Has(data as StatsData))
            {
                availableUpgrades.Add(data);
            }
        }
    }

    foreach (UpgradeUI upgradeOption in upgradeUIOpitons)
    {
        if (availableUpgrades.Count <= 0)
        {
            return;
        }

        ItemData choosingUpgrade = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];
        availableUpgrades.Remove(choosingUpgrade);

        if (choosingUpgrade != null)
        {
            EnableUpgradeUI(upgradeOption);

            Item item = Get(choosingUpgrade);
            if (item)
            {
                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUp(item));

                Item.LevelData nextLevel;
                if (item is Weapon)
                {
                    nextLevel = ((WeaponData)choosingUpgrade).GetLevelData(item.currentLevel + 1);
                    upgradeOption.upgradeTypeDisplay.text = "Weapon";
                }
                else if (item is Passive)
                {
                    nextLevel = ((PassiveData)choosingUpgrade).GetLevelData(item.currentLevel + 1);
                    upgradeOption.upgradeTypeDisplay.text = "Passive";
                }
                else if (item is Stats)
                {
                    nextLevel = ((StatsData)choosingUpgrade).GetLevelData(item.currentLevel + 1);
                    upgradeOption.upgradeTypeDisplay.text = "Stats";
                }
                else
                {
                    Debug.LogError("Unknown item type");
                    return;
                }

                upgradeOption.upgradeDescripionDisplay.text = nextLevel.description;
                upgradeOption.upgradeIcon.sprite = nextLevel.Icon;
                upgradeOption.upgradeNameDisplay.text = nextLevel.name;
            }
            else
            {
                if (choosingUpgrade is WeaponData w)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => Add(w));
                    upgradeOption.upgradeDescripionDisplay.text = w.baseStats.description;
                    upgradeOption.upgradeIcon.sprite = w.baseStats.Icon;
                    upgradeOption.upgradeNameDisplay.text = w.baseStats.name;
                    upgradeOption.upgradeTypeDisplay.text = "Weapon";
                }
                else if (choosingUpgrade is PassiveData p)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => Add(p));
                    upgradeOption.upgradeDescripionDisplay.text = p.baseStats.description;
                    upgradeOption.upgradeIcon.sprite = p.baseStats.Icon;
                    upgradeOption.upgradeNameDisplay.text = p.baseStats.name;
                    upgradeOption.upgradeTypeDisplay.text = "Passive";
                }
                else if (choosingUpgrade is StatsData s)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => Add(s));
                    upgradeOption.upgradeDescripionDisplay.text = s.baseStats.description;
                    upgradeOption.upgradeIcon.sprite = s.baseStats.Icon;
                    upgradeOption.upgradeNameDisplay.text = s.baseStats.name;
                    upgradeOption.upgradeTypeDisplay.text = "Stats";
                }
            }
        }
    }
}

  // Remove all upgrade options from the UI
    public void RemoveUpgradeOption()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOpitons)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgrade(upgradeOption);
        }
    }
    // Remove current upgrade options and apply new ones
    public void RemoveAndApplyUpgradeOption()
    {
        RemoveUpgradeOption();
        ApplyUpgradeOptions();
    }
    // Disable a specific upgrade UI element
    public void DisableUpgrade(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }
    // Enable a specific upgrade UI element
    public void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}