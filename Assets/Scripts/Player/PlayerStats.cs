using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class PlayerStats : MonoBehaviour
{
    [Header("Character Data")]
    public static PlayerStats instance;
    public CharacterData cst;
    [HideInInspector]
    public float[] position;
    public CharacterData.Stats baseStat;
    [SerializeField]
    public CharacterData.Stats actualStats;
    public CharacterData.Stats Stats
    {
        get{return actualStats;}
        set{
            actualStats = value;
        }
    }

    PlayerPickUp playerPickUp;
    float currentHeal;

    #region Current Stats Properties
    public float CurrentHeal
    {
        get { return currentHeal; }
        set
        {
            if(currentHeal != value){
            currentHeal = value;
            // if (GameManager.instance != null)
            // {
            //     GameManager.instance.CurrentHealDisplay.text = string.Format("Health: {0} / {1}",currentHeal, actualStats.maxHeal);
            // }
            UpdateHealDisplay();
            }
        }
    }
    #endregion

    [Header("Visual")]
    public ParticleSystem dameEffect;
    public ParticleSystem blockEffect;

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    

    public SpriteRenderer spriteRenderer;
    public Animator animator; 
    
    public int score = 0;
    public int coin = 0;
    public int killnumber = 0;

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
    [HideInInspector]
    public PlayerInventory playerInventory;

    [Header("UI")]
    public UnityEngine.UI.Image healBar;
    public UnityEngine.UI.Image ExpBar;
    public TMP_Text levelDisplay;
    public TMP_Text healDisplay;
    public TMP_Text coinDisplay;
    public TMP_Text killnumberDisplay;
    PlayerData playerData;
    GameManager gameManager;
    DataPersistenceManager dataPersistenceManager;
    CostumeData costumeData;

    private void Awake()
    {
                if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameManager = FindObjectOfType<GameManager>();
            InitializeNewGame();
            Debug.Log("New Game Data");
    }


public void InitializeNewGame()
{
    // Khởi tạo cho trò chơi mới
    // cst = CharacterSelected.GetData();
    cst = CharacterSelected.instance.characterData;
    costumeData = CharacterSelected.GetSelectedCostume();
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();

    // Ensure CharacterSelected instance exists and has a valid costumeData
    if (CharacterSelected.instance != null && CharacterSelected.instance.costumeData != null)
    {
        // Use selectedCostume to set sprite and animator
        spriteRenderer.sprite = costumeData.CostumeSprite;
        animator.runtimeAnimatorController = costumeData.CostumeAnimator;
    }
    else
    {
        // Fallback to default character data if no costume is selected
        spriteRenderer.sprite = cst.sprite;
        animator.runtimeAnimatorController = cst.animatorController;
        Debug.LogWarning("No costume selected, using default character appearance.");
    }

    
    if (CharacterSelected.instance != null && CharacterSelected.instance.gameObject.activeInHierarchy)
    {
        CharacterSelected.instance.DestroyInstance();
    }

    playerInventory = GetComponent<PlayerInventory>();
    playerPickUp = GetComponentInChildren<PlayerPickUp>();
    baseStat = actualStats = cst.stats;
    playerPickUp.SetMagnet(actualStats.magnet);
    currentHeal = actualStats.maxHeal;

    // Thêm vũ khí khởi đầu vào kho người chơi
    playerInventory.availableWeapons.Insert(0, cst.StartingWeapon);
    playerInventory.Add(cst.StartingWeapon);

    experienceCap = levelRanges[0].experienceCapIncrese;

    gameManager.AssignCharacter(cst);
    gameManager.Icon(cst);
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
        Recover();
        updateLevelDisplay();
    }


    public void RecalculatedStats()
    {
        actualStats = baseStat;
        foreach(PlayerInventory.Slot s in playerInventory.passiveSlot)
        {
            Passive p = s.item as Passive;
            if(p)
            {
                actualStats += p.GetBoots();
                Debug.Log("Cộng thông số");
            }
        }
        playerPickUp.SetMagnet(actualStats.magnet);
    }

    public void ApplyStatsUpgrade(){
         actualStats = baseStat;
        foreach(PlayerInventory.StatSlot s in playerInventory.statSlot)
        {
            Stats p = s.item as Stats;
            if(p)
            {
                actualStats += p.GetBoots();
                Debug.Log("Cộng thông số");
            }
        }
        playerPickUp.SetMagnet(actualStats.magnet);
    }

    public void IncreaseExperience(int amount)
    {
        int adjustedAmount = Mathf.CeilToInt(amount * actualStats.expMultiplier);
        // Debug.Log("Exp: " + adjustedAmount);
        // Debug.Log("ExpMultiple: " + cst.stats.expMultiplier);
        experience += adjustedAmount;
        levelCheckerUp();
        updateExpBar();
    }

    public void IncreaseHeal(float heal)
    {
        if (CurrentHeal < cst.stats.maxHeal)
        {
            CurrentHeal += heal*actualStats.healMultiplier;
            if (CurrentHeal > cst.stats.maxHeal)
            {
                CurrentHeal = cst.stats.maxHeal;
            }
            updateHealBar();
        }
    }
    public void IncreaseCoin(int coin)
    {
        this.coin += coin;
        UpdateCoinDisplay();
    }
    public void IncreaseKillnumber(int killnumber)
    {
        this.killnumber += killnumber;
        killnumberDisplay.text = "Kill: " + this.killnumber.ToString();
    }

    // public void levelCheckerUp()
    // {
    //     if (experience >= experienceCap)
    //     {
    //         level++;
    //         experience -= experienceCap;
    //         int experienceCapIncrese = 0;

    //         foreach (levelRange range in levelRanges)
    //         {
    //             if (level >= range.startLevel && level <= range.endLevel)
    //             {
    //                 experienceCapIncrese = range.experienceCapIncrese;
    //                 break;
    //             }
    //         }
    //         experienceCap += experienceCapIncrese;
    //         GameManager.instance.StartLevelUp();
    //         updateLevelDisplay();
    //         if(experience >= experienceCap) levelCheckerUp();
    //     }
    // }
        private void SetExperienceCapForLevel(int currentLevel)
    {
        foreach (levelRange range in levelRanges)
        {
            if (currentLevel >= range.startLevel && currentLevel <= range.endLevel)
            {
                experienceCap = range.experienceCapIncrese;
                break;
            }
        }
    }

    public void levelCheckerUp()
    {
        while (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;
            
            // Set experience cap for new level
            SetExperienceCapForLevel(level);

            // Ensure the level-up process is handled correctly
            if (GameManager.instance != null)
            {
                GameManager.instance.StartLevelUp();
            }
            else
            {
                Debug.LogWarning("GameManager instance is null, cannot start level up.");
            }

            updateLevelDisplay();
            // if(playerInventory.isInventoryFullAndMaxLevel == true)
            // {
            //     if (GameManager.instance.LevelUpScreen.activeSelf)
            // {
            //     GameManager.instance.EndLevelUp();
            // }
            // }
        }
    }

    public void TakeDamage(float dmg)
    {
        if (isInvicible == false)
        {
            dmg -= actualStats.armor;
            if(dmg > 0)
            {
                CurrentHeal -= dmg;
                if(dameEffect) Destroy(Instantiate(dameEffect,transform.position,Quaternion.identity),5f);
                invicibleTime = invicibleDuration;
                isInvicible = true;
                if (CurrentHeal <= 0)
                {
                    Kill();
                }
            }
            else
            {
                if(blockEffect) Destroy(Instantiate(dameEffect,transform.position,Quaternion.identity),5f);
            }
            updateHealBar();
        }
    }

    public void updateHealBar()
    {
        // Ensure that healBar is not null to avoid errors
        if (healBar != null)
        {
            healBar.fillAmount = currentHeal / actualStats.maxHeal;
        }
        UpdateHealDisplay();
    }

public void updateExpBar()
{
    // Ensure that ExpBar is not null and experienceCap is not zero to avoid errors
    if (ExpBar != null && experienceCap > 0)
    {
        ExpBar.fillAmount = (float)experience / experienceCap;
    }
    else if (ExpBar != null)
    {
        ExpBar.fillAmount = 0f; // Or some other default value
    }
}


    public void updateLevelDisplay()
    {
        // Ensure that levelDisplay is not null to avoid errors
        if (levelDisplay != null)
        {
            levelDisplay.text = "LV:" + level.ToString();
        }
    }
    public void UpdateCoinDisplay()
    {
        // Ensure that coinDisplay is not null to avoid errors
        if (coinDisplay != null)
        {
            coinDisplay.text = "Coin: " + coin.ToString();
        }
    }

    public void RestoreHeal(float amount)
    {
        if(CurrentHeal < actualStats.maxHeal)
        {
            CurrentHeal += amount*actualStats.healMultiplier;
            if(CurrentHeal > actualStats.maxHeal)
            {
                CurrentHeal = actualStats.maxHeal;
            }
        }
        updateHealBar();
    }

    public void Recover()
    {
        if(CurrentHeal < actualStats.maxHeal)
        {
            CurrentHeal += Stats.recovery*Time.deltaTime * actualStats.healMultiplier;
            if(CurrentHeal > actualStats.maxHeal)
            {
                CurrentHeal = actualStats.maxHeal;
            }
        }
        updateHealBar();
    }

    public void Kill()
    {
        if (!GameManager.instance.IsGameOver)
        {
            GameManager.instance.AssignLevel(level);
            GameManager.instance.AssignScore(score);
            GameManager.instance.AssignWeaponAndPassiveItem(playerInventory.weaponSlot, playerInventory.passiveSlot);
            GameManager.instance.GameOver();
        }
    }

    public void PlusScore()
    {
        score += 10;
    }

    private void UpdateHealDisplay()
    {
        // Ensure that healDisplay is not null to avoid errors
        if (healDisplay != null)
        {
            healDisplay.text = $"{currentHeal} / {actualStats.maxHeal}";
        }
    }
}
