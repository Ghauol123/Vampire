using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject cst;
    public WeaponScriptableObject wst;
    public float currentHeal;
    public float CurrentHeal
    {
        get { return currentHeal; }
        set
        {
            currentHeal = value;
            if (GameManager.instance != null)
            {
                GameManager.instance.CurrentHealDisplay.text = "Heal:" + currentHeal;
            }
        }
    }
    public float currentRecovery;
    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            currentRecovery = value;
            if (GameManager.instance != null)
            {
                GameManager.instance.CurrentrecoveryDisplay.text = "Recovery:" + currentRecovery;
            }
        }
    }
    public float currentSpeed;
    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set
        {
            currentSpeed = value;
            if (GameManager.instance != null)
            {
                GameManager.instance.CurrentSpeedDisplay.text = "Speed:" + currentSpeed;
            }
        }
    }
    float currentMight;
    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            currentMight = value;
            if (GameManager.instance != null)
            {
                GameManager.instance.CurrentMightDisplay.text = "Might:" + currentMight;
            }
        }
    }
    float currentProjectileSpeed;
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            currentProjectileSpeed = value;
            if (GameManager.instance != null)
            {
                GameManager.instance.CurrentProjectTileSpeedDisplay.text = "ProjectileSpeed:" + currentProjectileSpeed;
            }
        }
    }
    float currentManget;
    public float CurrentManget
    {
        get { return currentManget; }
        set
        {
            currentManget = value;
            if (GameManager.instance != null)
            {
                GameManager.instance.CurrentMagnetDisplay.text = "Magnet:" + currentManget;
            }
        }
    }
    // public List<GameObject> startWeapon;

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    public SpriteRenderer spriteRenderer;
    public Animator animator;
    InventoryManager inventoryManager;
    public int weaponIndex;
    public int passiveItemsIndex;
    public GameObject weaponTest;
    public GameObject passiveItemsText;
    public GameObject passiveItemsText2;
    public int score = 0;

    [System.Serializable]
    public class levelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrese;
    }
    [Header("I-Frames")]
    public float invicibleDuration;
    float invicibleTime;
    bool isInvicible;
    public List<levelRange> levelRanges;
    [Header("UI")]
    public Image healBar;
    public Image ExpBar;
    public TMP_Text levelDisplay;
    public TMP_Text healDisplay;
    // public int characterIndex;
    // public GameObject[] playerPrefabs;
    // public static Vector2 lastCheckPointPos = new Vector2(-3,0);
    private void Awake()
    {

        // characterIndex =  PlayerPrefs.GetInt("SelectedCharacter", 0);
        // playerAnimation = playerPrefabs[characterIndex].GetComponent<RuntimeAnimatorController>();
        // spr = playerPrefabs[characterIndex].GetComponent<SpriteRenderer>();
        cst = CharacterSelected.GetData();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer.sprite = cst.sprite;
        animator.runtimeAnimatorController = cst.animatorController;

        CharacterSelected.instance.DestroyInstance();
        inventoryManager = GetComponent<InventoryManager>();
        CurrentHeal = cst.Maxheal;
        currentMight = cst.Might;
        currentProjectileSpeed = cst.ProjectileSpeed;
        currentRecovery = cst.Recovery;
        currentSpeed = cst.MoveSpeed;
        currentManget = cst.Magnet;
        // newPosition = new Vector2(transform.position.x, -0.70f);
        SpawnWeapon(cst.StartingWeapon);
        SpawnPassiveItems(passiveItemsText);
        // SpawnPassiveItems(passiveItemsText2);
        // SpawnWeapon(weaponTest);
    }
    private void Start()
    {


        experienceCap = levelRanges[0].experienceCapIncrese;
        // đặt max kinh nghiệp đầu tiên là max kinh nghiệm ở level 0
        // playerAnimation = GetComponent<Animator>();
        // playerAnimation.runtimeAnimatorController = cst._Animation;
        // spr = GetComponent<SpriteRenderer>();
        // spr.sprite = cst.Image;
        GameManager.instance.CurrentHealDisplay.text = "Heal:" + currentHeal;
        GameManager.instance.CurrentrecoveryDisplay.text = "Recovery:" + currentRecovery;
        GameManager.instance.CurrentSpeedDisplay.text = "Speed:" + currentSpeed;
        GameManager.instance.CurrentMightDisplay.text = "Might:" + currentMight;
        GameManager.instance.CurrentProjectTileSpeedDisplay.text = "ProjectileSpeed:" + currentProjectileSpeed;
        GameManager.instance.CurrentMagnetDisplay.text = "Magnet:" + currentManget;

        GameManager.instance.AssignCharacter(cst);
        GameManager.instance.Icon(cst);
        updateHealBar();
        updateExpBar();
        updateLevelDisplay();
    }
    private void Update()
    {
        if (invicibleTime > 0)
        {
            invicibleTime -= Time.deltaTime;

        }
        else if (isInvicible)
        {
            isInvicible = false;
        }
        Recovery();
    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        levelCheckerUp();
        updateExpBar();
    }
    public void IncreaseHeal(float heal)
    {
        if (CurrentHeal < cst.Maxheal)
        {
            CurrentHeal += heal;
            if (CurrentHeal > cst.Maxheal)
            {
                CurrentHeal = cst.Maxheal;
            }
            updateHealBar();
        }
    }
    public void Recovery()
    {
        if (CurrentHeal < cst.Maxheal)
        {
            CurrentHeal += currentRecovery * Time.deltaTime;
            if (CurrentHeal > cst.Maxheal)
            {
                CurrentHeal = cst.Maxheal;
            }
            updateHealBar();
        }
    }
    public void levelCheckerUp()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;
            int experienceCapIncrese = 0;
            foreach (levelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrese = range.experienceCapIncrese;
                    break;
                }
            }
            experienceCap += experienceCapIncrese;
            GameManager.instance.StartLevelUp();
            updateLevelDisplay();
        }
    }
    public void TakeDamage(float dmg)
    {
        if (isInvicible == false)
        {
            CurrentHeal -= dmg;
            invicibleTime = invicibleDuration;
            isInvicible = true;
            if (CurrentHeal <= 0)
            {
                Kill();
            }
            updateHealBar();
        }
    }
    void updateHealBar()
    {
        healBar.fillAmount = currentHeal / cst.Maxheal;
        // healDisplay.text = (currentHeal/cst.Maxheal).ToString();
    }
    void updateExpBar(){
        ExpBar.fillAmount = (float)experience/experienceCap;
    }
    void updateLevelDisplay(){
        levelDisplay.text = "LV:" + level.ToString();
    }
    public void Kill()
    {
        if (!GameManager.instance.IsGameOver)
        {
            GameManager.instance.AssignLevel(level);
            GameManager.instance.AssignScore(score);
            GameManager.instance.AssignWeaponAndPassiveItem(inventoryManager.weaponImageSlot, inventoryManager.passiveItemImageSlot);
            GameManager.instance.GameOver();
        }
    }
    public void PlusScore()
    {
        score += 10;
    }
    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex > inventoryManager.weaponSlot.Count - 1)// trừ 1 vì bắt đầu từ 0;
        {
            Debug.Log("Full weapon");
            return;
        }
        // Vector3 newPosition = new Vector3();
        // newPosition = new Vector3(transform.position.x, -0.40f, transform.position.z);

        // Instantiate vật phẩm với vị trí mới
        GameObject spawnWeapon = Instantiate(weapon, transform.position, Quaternion.identity);

        spawnWeapon.transform.SetParent(transform);
        // startWeapon.Add(spawnWeapon);
        inventoryManager.AddWeapon(weaponIndex, spawnWeapon.GetComponent<WeaponController>()); // thêm vũ khí vào inventory

        weaponIndex++;
    }
    public void SpawnPassiveItems(GameObject passiveItems)
    {
        if (passiveItemsIndex > inventoryManager.passiveItemsSlot.Count - 1)// trừ 1 vì bắt đầu từ 0;
        {
            Debug.Log("Full weapon");
            return;
        }
        GameObject spawnPassiveItems = Instantiate(passiveItems, transform.position, Quaternion.identity);
        spawnPassiveItems.transform.SetParent(transform);
        // startWeapon.Add(spawnWeapon);
        inventoryManager.AddPassiveItem(passiveItemsIndex, spawnPassiveItems.GetComponent<PassiveItems>()); // thêm vũ khí vào inventory

        passiveItemsIndex++;
    }

}
