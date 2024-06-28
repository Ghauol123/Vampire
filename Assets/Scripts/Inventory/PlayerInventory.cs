using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using JetBrains.Annotations;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot{
        public Item item;
        public UnityEngine.UI.Image image;
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
        public void Clear(){
            item = null;
            image.enabled = false;
            image.sprite = null;
        }
        public bool IstEmpty() { return item == null;}
    }
    public List<Slot> weaponSlot = new List<Slot>(6);
    public List<Slot> passiveSlot = new List<Slot>(6);
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
    public List<UpgradeUI> upgradeUIOpitons = new List<UpgradeUI>();
    PlayerStats playerStats;
    private void Start() {
        playerStats = FindAnyObjectByType<PlayerStats>();
        // newUpgrade.initialWeapon = playerStats.cst.StartingWeapon;
        // weaponData = playerStats.cst.StartingWeapon;
        // Thêm đối tượng mới vào vị trí đầu tiên trong danh sách
        availableWeapons.Insert(0, playerStats.cst.StartingWeapon);
    }
    //Check if the inventory has an item of a certaint type;
    public bool Has(ItemData type) {return Get(type);}
    public Item Get(ItemData type){
        //Determine what type of item we are looking for
        if(type is WeaponData) return Get(type as WeaponData);
        else if(type is PassiveData) return Get(type as PassiveData);
        return null;
    }
    //Find a passive of a certain type in the inventory
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
    //Find a weapon of a certain type in the inventory
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
    //Remove a weapon of a particular type, as specifield by <data>
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
    // if an ItemData is passed, determined what type it is and call the respective overload.
    // We aso have an optional boolean to remove this item from the upgrade list.
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
    // if an ItemData is passed, determined what type it is and call the respective overload.
    // We aso have an optional boolean to remove this item from the upgrade list.
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
    
    //if we don't know what item is beig added, this function will determine that.
    public int Add(ItemData data){
        if(data is WeaponData) return Add(data as WeaponData);
        else if(data is PassiveData) return Add(data as PassiveData);
        return -1;
    }
    public bool LevelUp(ItemData data){
        Item item = Get(data);
        if(item) return LevelUp(item);
        return false;
    }
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
        return true;
    }
    int GetSlotLeft(List<Slot> slots){
        int count = 0;
        foreach(Slot s in slots){
            if(s.IstEmpty()) count++;
        }
        return count;
    }
    // public void LevelUpWeapon(int slotIndex, int upgradeIndex){
    //     if(weaponSlot.Count > slotIndex){
    //         Weapon weapon = weaponSlot[slotIndex].item as Weapon;
    //         //Don't level up the weapon if it is already at max level
    //         if(!weapon.DoLevelUp()){
    //             Debug.LogWarning("Failed to level up" + weapon.name);
    //             return;
    //         }
    //     }
    //     if(GameManager.instance != null && GameManager.instance.chosingUpgrade){
    //         GameManager.instance.EndLevelUp();
    //     }
    // }
    // public void LevelUpPassiveItem(int slotIndex, int upgradeIndex){
    //     if(passiveSlot.Count > slotIndex){
    //         Passive passive = passiveSlot[slotIndex].item as Passive;
    //         //Don't level up the passive if it is already at max level
    //         if(!passive.DoLevelUp()){
    //             Debug.LogWarning("Failed to level up" + passive.name);
    //             return;
    //         }
    //     }
    //     if(GameManager.instance != null && GameManager.instance.chosingUpgrade){
    //         GameManager.instance.EndLevelUp();
    //     }
    //     playerStats.RecalculatedStats();
    // }
//     void ApplyUpgradeOptions(){
//         //Make a duplicatio of the available weapon / passive upgrade lists
//         // so we can iterate through them in the function

//         List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
//         List<PassiveData> availablePassiveUpgrades = new List<PassiveData>(availablePassive);
//         //Iterate through each slot in the upgrade UI
//         foreach(UpgradeUI upgradeOption in upgradeUIOpitons){
//             //if there are no more available upgrades, then we abort.
//             if(availableWeaponUpgrades.Count == 0 && availablePassiveUpgrades.Count == 0){
//                 return;
//             }
//             // Determine whether this upgrade should be for passive or active weapons
//             int upgradeType;
//             if (availableWeaponUpgrades.Count == 0)
//             {
//                 upgradeType = 2;
//             }
//             else if (availablePassiveUpgrades.Count == 0)
//             {
//                 upgradeType = 1;
//             }
//             else
//             {
//                 upgradeType = UnityEngine.Random.Range(1, 3);
//             }
//              if (upgradeType == 1)
//             {
//                 WeaponData choosingWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
//                 availableWeaponUpgrades.Remove(choosingWeaponUpgrade);
//                 if(choosingWeaponUpgrade != null){
//                     //turns on the UI slot
//                     EnableUpgradeUI(upgradeOption);
//                     //Loops through all our existing weapons. If we find a match, we will
//                     // hook a event listener to the button that will level up the weapon
//                     // When this upgrade option is clicked
//                     bool isLevelup = false;
//                   for(int i = 0; i < weaponSlot.Count; i++){
//                     Weapon w = weaponSlot[i].item as Weapon;
//                     if(w != null && w.data == choosingWeaponUpgrade){
//                             // If the weapon is already at the max level, do not allow upgrades
//                             if(choosingWeaponUpgrade.maxLevel <= w.currentLevel){
//                                 DisableUpgrade(upgradeOption);
//                                 isLevelup = true;
//                                 break;
//                             }
//                             // Set the Event Listener, item and level description to be that of the next level
//                             upgradeOption.upgradeButton.onClick.AddListener(() => LevelUp(w)); // Apply button functionality
//                             Weapon.Stats nextLevel = choosingWeaponUpgrade.GetLevelData(w.currentLevel + 1);
//                             upgradeOption.upgradeDescripionDisplay.text = nextLevel.description;
//                             upgradeOption.upgradeIcon.sprite = nextLevel.Icon;
//                             upgradeOption.upgradeNameDisplay.text = nextLevel.name;
//                             upgradeOption.upgradeTypeDisplay.text = nextLevel.type;
//                             isLevelup = true;
//                             break;
//                         }
//                     }
//                     //if the code gets here, it means that we will be adding a new weapon, instead of
//                     // upgrading an existing weapon.
//                     if(!isLevelup){
//                             upgradeOption.upgradeButton.onClick.AddListener(() => Add(choosingWeaponUpgrade)); // Apply button functionality
//                             upgradeOption.upgradeDescripionDisplay.text = choosingWeaponUpgrade.baseStats.description;
//                             upgradeOption.upgradeIcon.sprite = choosingWeaponUpgrade.baseStats.Icon;
//                             upgradeOption.upgradeNameDisplay.text = choosingWeaponUpgrade.baseStats.name;
//                             upgradeOption.upgradeTypeDisplay.text = choosingWeaponUpgrade.baseStats.type;
//                     }
//                 }
//             }
//             else if(upgradeType ==2 ){
//                 // Note: We have to recode this system, as right now it disable an upgrade slot if
//                 // we hit a weapon that has already reached max level
//                 PassiveData chosenPassiveUpgrade = availablePassiveUpgrades[UnityEngine.Random.Range(0,availablePassiveUpgrades.Count)];
//                 availablePassiveUpgrades.Remove(chosenPassiveUpgrade);
//                 if(chosenPassiveUpgrade != null){
//                     EnableUpgradeUI(upgradeOption);
//                     bool isLevelUp = false;
//                     for(int i = 0; i<availablePassive.Count; i++){
//                         Passive p = passiveSlot[i].item as Passive;
//                         if(p != null && p.data == chosenPassiveUpgrade){
//                             if(chosenPassiveUpgrade.maxLevel <= p.currentLevel){
//                                 DisableUpgrade(upgradeOption);
//                                 isLevelUp = true;
//                                 break;
//                             }
//                             upgradeOption.upgradeButton.onClick.AddListener(() => LevelUp(p)); // Apply button functionality
//                             Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
//                             upgradeOption.upgradeDescripionDisplay.text = nextLevel.description;
//                             upgradeOption.upgradeIcon.sprite = nextLevel.Icon;
//                             upgradeOption.upgradeNameDisplay.text = nextLevel.name;
//                             upgradeOption.upgradeTypeDisplay.text = nextLevel.type;
//                             isLevelUp = true;
//                             break;
//                         }
//                     }
//                     if(!isLevelUp){
//                             upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade)); // Apply button functionality
//                             upgradeOption.upgradeDescripionDisplay.text = chosenPassiveUpgrade.baseStats.description;
//                             upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.baseStats.Icon;
//                             upgradeOption.upgradeNameDisplay.text = chosenPassiveUpgrade.baseStats.name;
//                             upgradeOption.upgradeTypeDisplay.text = chosenPassiveUpgrade.baseStats.type;
//                     }
//                 }
//             }
//     }
// }
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

    // Iterate through each slot in the upgrade UI
    int weaponSlotList = GetSlotLeft(weaponSlot);
    int passiveSlotList = GetSlotLeft(passiveSlot);

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
            if (data is WeaponData && weaponSlotList > 0)
            {
                availableUpgrades.Add(data);
            }
            else if (data is PassiveData && passiveSlotList > 0)
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

                if (item is Weapon)
                {
                    Weapon w = item as Weapon;
                    Weapon.Stats nextLevel = ((WeaponData)choosingUpgrade).GetLevelData(item.currentLevel + 1);
                    upgradeOption.upgradeDescripionDisplay.text = nextLevel.description;
                    upgradeOption.upgradeIcon.sprite = nextLevel.Icon;
                    upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                    upgradeOption.upgradeTypeDisplay.text = nextLevel.type;
                }
                else if (item is Passive)
                {
                    Passive p = item as Passive;
                    Passive.Modifier nextLevel = ((PassiveData)choosingUpgrade).GetLevelData(item.currentLevel + 1);
                    upgradeOption.upgradeDescripionDisplay.text = nextLevel.description;
                    upgradeOption.upgradeIcon.sprite = nextLevel.Icon;
                    upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                    upgradeOption.upgradeTypeDisplay.text = nextLevel.type;
                }
            }
            else
            {
                if (choosingUpgrade is WeaponData)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => Add((WeaponData)choosingUpgrade));
                    WeaponData w = choosingUpgrade as WeaponData;
                    upgradeOption.upgradeDescripionDisplay.text = w.baseStats.description;
                    upgradeOption.upgradeIcon.sprite = w.baseStats.Icon;
                    upgradeOption.upgradeNameDisplay.text = w.baseStats.name;
                    upgradeOption.upgradeTypeDisplay.text = w.baseStats.type;
                }
                else if (choosingUpgrade is PassiveData)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => Add((PassiveData)choosingUpgrade));
                    PassiveData p = choosingUpgrade as PassiveData;
                    upgradeOption.upgradeDescripionDisplay.text = p.baseStats.description;
                    upgradeOption.upgradeIcon.sprite = p.baseStats.Icon;
                    upgradeOption.upgradeNameDisplay.text = p.baseStats.name;
                    upgradeOption.upgradeTypeDisplay.text = p.baseStats.type;
                }
            }
        }
    }
}

  public void RemoveUpgradeOption()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOpitons)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgrade(upgradeOption);
        }
    }
    public void RemoveAndApplyUpgradeOption()
    {
        RemoveUpgradeOption();
        ApplyUpgradeOptions();
    }
    public void DisableUpgrade(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }
    public void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}
