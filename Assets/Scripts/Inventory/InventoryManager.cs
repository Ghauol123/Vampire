using System.Collections;
using System.Collections.Generic;
// using System.Security.Cryptography;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;



public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlot = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponImageSlot = new List<Image>(6);
    public List<PassiveItems> passiveItemsSlot = new List<PassiveItems>(6);
    public int[] passiveItemsLevels = new int[6];
    public List<Image> passiveItemImageSlot = new List<Image>(6);

    PlayerStats playerStats;
    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon; // vu khi hien tai
        public WeaponScriptableObject weaponScriptableObject;
    }
    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem; // vu khi hien tai
        public PassiveItemsScriptableObject passiveItemsScriptableObject;
    }
    [System.Serializable]
    public class UpgradeUI
    {
        public Text upgradeNameDisplay;
        public Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Text upgradeType;
        public Button upgradeButton;
    }
    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>();
    public List<UpgradeUI> upgradeUI = new List<UpgradeUI>();

    public void AddWeapon(int slotIndex, WeaponController weaponController)
    {
        weaponSlot[slotIndex] = weaponController;
        weaponLevels[slotIndex] = weaponController.wst.Level;
        weaponImageSlot[slotIndex].enabled = true;
        weaponImageSlot[slotIndex].sprite = weaponController.wst.Icon;
        if (GameManager.instance != null && GameManager.instance.chosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    public void AddPassiveItem(int slotIndex, PassiveItems passiveItems)
    {
        passiveItemsSlot[slotIndex] = passiveItems;
        passiveItemsLevels[slotIndex] = passiveItems.passiveItemsScriptableObject.Level;
        passiveItemImageSlot[slotIndex].enabled = true;
        passiveItemImageSlot[slotIndex].sprite = passiveItems.passiveItemsScriptableObject.Icon;
        if (GameManager.instance != null && GameManager.instance.chosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    public void LevelWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlot.Count > slotIndex)
        {
            WeaponController weapon = weaponSlot[slotIndex];
            if (!weapon.wst.NextLevelPrefabs)
            {
                Debug.LogWarning("FUll");
                return;
            }
            GameObject upgradeWeapon = Instantiate(weapon.wst.NextLevelPrefabs, new Vector2(playerStats.transform.position.x, playerStats.transform.position.y - 0.40f), Quaternion.identity);
            upgradeWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradeWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradeWeapon.GetComponent<WeaponController>().wst.Level;
            weaponUpgradeOptions[upgradeIndex].weaponScriptableObject = upgradeWeapon.GetComponent<WeaponController>().wst;
            if (GameManager.instance != null && GameManager.instance.chosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }
    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveItemsSlot.Count > slotIndex)
        {
            PassiveItems passiveItems = passiveItemsSlot[slotIndex];
            if (!passiveItems.passiveItemsScriptableObject.NextLevelPrefabs)
            {
                Debug.LogWarning("FUll");
                return;
            }
            GameObject upgradePassiveItems = Instantiate(passiveItems.passiveItemsScriptableObject.NextLevelPrefabs, playerStats.transform.position, Quaternion.identity);
            upgradePassiveItems.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradePassiveItems.GetComponent<PassiveItems>());
            Destroy(passiveItems.gameObject);
            passiveItemsLevels[slotIndex] = upgradePassiveItems.GetComponent<PassiveItems>().passiveItemsScriptableObject.Level;
            passiveItemUpgradeOptions[upgradeIndex].passiveItemsScriptableObject = upgradePassiveItems.GetComponent<PassiveItems>().passiveItemsScriptableObject;
            if (GameManager.instance != null && GameManager.instance.chosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeapon = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItem = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);
        foreach (var upgradeOption in upgradeUI)
        {
            if (availableWeapon.Count == 0 && availablePassiveItem.Count == 0)
            {
                return;
            }
            int upgradeType;
            if (availablePassiveItem.Count == 0)
            {
                upgradeType = 1;
            }
            else if (availableWeapon.Count == 0)
            {
                upgradeType = 2;
            }
            else
            {
                upgradeType = Random.Range(1, 3);
            }
            if (upgradeType == 1)
            {
                WeaponUpgrade choosingWeaponUpgrade = availableWeapon[Random.Range(0, availableWeapon.Count)];
                availableWeapon.Remove(choosingWeaponUpgrade);
                if (choosingWeaponUpgrade != null)
                {
                    EnableUpgrade(upgradeOption);
                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlot.Count; i++)
                    {
                        if (weaponSlot[i] != null && weaponSlot[i].wst == choosingWeaponUpgrade.weaponScriptableObject)
                        {
                            newWeapon = false;
                            if (!newWeapon)
                            {
                                if (!choosingWeaponUpgrade.weaponScriptableObject.NextLevelPrefabs)
                                {
                                    DisableUpgrade(upgradeOption);
                                    break;
                                }
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelWeapon(i, choosingWeaponUpgrade.weaponUpgradeIndex));
                                upgradeOption.upgradeDescriptionDisplay.text = choosingWeaponUpgrade.weaponScriptableObject.NextLevelPrefabs.GetComponent<WeaponController>().wst.Description;
                                upgradeOption.upgradeNameDisplay.text = choosingWeaponUpgrade.weaponScriptableObject.NextLevelPrefabs.GetComponent<WeaponController>().wst.Name;
                                upgradeOption.upgradeType.text = choosingWeaponUpgrade.weaponScriptableObject.NextLevelPrefabs.GetComponent<WeaponController>().wst.Type;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if (newWeapon)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => playerStats.SpawnWeapon(choosingWeaponUpgrade.initialWeapon));
                        upgradeOption.upgradeDescriptionDisplay.text = choosingWeaponUpgrade.weaponScriptableObject.Description;
                        upgradeOption.upgradeNameDisplay.text = choosingWeaponUpgrade.weaponScriptableObject.Name;
                        upgradeOption.upgradeType.text = choosingWeaponUpgrade.weaponScriptableObject.Type;

                    }
                    upgradeOption.upgradeIcon.sprite = choosingWeaponUpgrade.weaponScriptableObject.Icon;
                }
            }
            else if (upgradeType == 2)
            {
                PassiveItemUpgrade choosingPassiveItemUpgrade = availablePassiveItem[Random.Range(0, availablePassiveItem.Count)];
                availablePassiveItem.Remove(choosingPassiveItemUpgrade);
                if (choosingPassiveItemUpgrade != null)
                {
                    EnableUpgrade(upgradeOption);
                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemsSlot.Count; i++)
                    {
                        if (passiveItemsSlot[i] != null && passiveItemsSlot[i].passiveItemsScriptableObject == choosingPassiveItemUpgrade.passiveItemsScriptableObject)
                        {
                            newPassiveItem = false;
                            if (!newPassiveItem)
                            {
                                if (!choosingPassiveItemUpgrade.passiveItemsScriptableObject.NextLevelPrefabs)
                                {
                                    DisableUpgrade(upgradeOption);
                                    break;
                                }
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, choosingPassiveItemUpgrade.passiveItemUpgradeIndex));
                                upgradeOption.upgradeDescriptionDisplay.text = choosingPassiveItemUpgrade.passiveItemsScriptableObject.NextLevelPrefabs.GetComponent<PassiveItems>().passiveItemsScriptableObject.Description;
                                upgradeOption.upgradeNameDisplay.text = choosingPassiveItemUpgrade.passiveItemsScriptableObject.NextLevelPrefabs.GetComponent<PassiveItems>().passiveItemsScriptableObject.Name;
                                upgradeOption.upgradeType.text = choosingPassiveItemUpgrade.passiveItemsScriptableObject.NextLevelPrefabs.GetComponent<PassiveItems>().passiveItemsScriptableObject.Type;
                            }
                            break;
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }
                    if (newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => playerStats.SpawnPassiveItems(choosingPassiveItemUpgrade.initialPassiveItem));
                        upgradeOption.upgradeDescriptionDisplay.text = choosingPassiveItemUpgrade.passiveItemsScriptableObject.Description;
                        upgradeOption.upgradeNameDisplay.text = choosingPassiveItemUpgrade.passiveItemsScriptableObject.Name;
                        upgradeOption.upgradeType.text = choosingPassiveItemUpgrade.passiveItemsScriptableObject.Type;


                    }
                    upgradeOption.upgradeIcon.sprite = choosingPassiveItemUpgrade.passiveItemsScriptableObject.Icon;
                }
            }
        }
    }
    public void RemoveUpgradeOption()
    {
        foreach (var upgradeOption in upgradeUI)
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
    public void DisableUpgrade(UpgradeUI ui){
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }
    public void EnableUpgrade(UpgradeUI ui){
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}

