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

    public void AddWeapon(int slotIndex, WeaponController weaponController)
    {
        weaponSlot[slotIndex] = weaponController;
        weaponLevels[slotIndex] = weaponController.wst.Level;
        weaponImageSlot[slotIndex].enabled = true;
        weaponImageSlot[slotIndex].sprite = weaponController.wst.Icon;
    }
    public void AddPassiveItem(int slotIndex, PassiveItems passiveItems)
    {
        passiveItemsSlot[slotIndex] = passiveItems;
        passiveItemsLevels[slotIndex] = passiveItems.passiveItemsScriptableObject.Level;
        passiveItemImageSlot[slotIndex].enabled = true;
        passiveItemImageSlot[slotIndex].sprite = passiveItems.passiveItemsScriptableObject.Icon;
    }
    public void LevelWeapon(int slotIndex)
    {
        if (weaponSlot.Count > slotIndex)
        {
            WeaponController weapon = weaponSlot[slotIndex];
            if (!weapon.wst.NextLevelPrefabs)
            {
                Debug.LogWarning("FUll");
                return;
            }
            GameObject upgradeWeapon = Instantiate(weapon.wst.NextLevelPrefabs, new Vector2(playerStats.transform.position.x, playerStats.transform.position.y-0.40f), Quaternion.identity);
            upgradeWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradeWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradeWeapon.GetComponent<WeaponController>().wst.Level;
        }
    }
    public void LevelUpPassiveItem(int slotIndex)
    {
        if (passiveItemsSlot.Count < slotIndex)
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
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
