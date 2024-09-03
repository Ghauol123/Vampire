using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class PlayerStats : MonoBehaviour, IDataPersistence
{
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
    PlayerData playerData;
    GameManager gameManager;
    DataPersistenceManager dataPersistenceManager;
    CostumeData costumeData;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        if(DataPersistenceManager.instance.isNewgame == true)
        {
            InitializeNewGame();
            Debug.Log("New Game Data");
        }
        else
        {
            LoadGameData(DataPersistenceManager.instance.gameData);
            Debug.Log("Load Game Data");
        }
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
public void LoadGameData(GameData gameData)
{
    PlayerStats playerStats = GetComponent<PlayerStats>();
    if (playerStats == null)
    {
        Debug.LogError("Thành phần PlayerStats bị thiếu!");
        return;
    }
            if (gameData == null)
        {
            Debug.LogWarning("gameData là null");
            return;
        }

    // Initialize other components
    playerPickUp = GetComponentInChildren<PlayerPickUp>();
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();
    playerInventory = GetComponent<PlayerInventory>();

    // Set basic stats and positions
    transform.position = gameData.playerPosition;
    baseStat = gameData.baseStat;
    actualStats = gameData.actualStat;
    level = gameData.level;
    experience = gameData.experience;
    currentHeal = gameData.currentHealth;
    score = gameData.score;
    gameManager.stopWatchTime = gameData.timeSurvival;
    actualStats.magnet = gameData.magnet;
    playerPickUp.SetMagnet(gameData.magnet);

    // Load sprites and animator controllers
    LoadSpriteAndAnimator(gameData);

    // Clear and set inventory
    playerInventory.ClearInventory();
    playerInventory.availableWeapons = new List<WeaponData>(gameData.availableWeapons);
    playerInventory.availablePassive = new List<PassiveData>(gameData.availablePassiveItems);
    
    // Add weapons and passives
    AddInventoryItems(gameData);
    SetExperienceCapForLevel(level);
    Debug.Log($"Experience Cap set to: {experienceCap} for Level: {level}");
    updateExpBar();
    LoadEnemiesAndWave(gameData);
}

private void LoadSpriteAndAnimator(GameData gameData)
{
    Sprite loadedSprite = Resources.Load<Sprite>(gameData.spriteRendererSpriteName);
    if (loadedSprite != null)
    {
        spriteRenderer.sprite = loadedSprite;
    }
    else
    {
        Debug.LogWarning("Sprite not found: " + gameData.spriteRendererSpriteName);
    }

    RuntimeAnimatorController loadedAnimatorController = Resources.Load<RuntimeAnimatorController>(gameData.animatorControllerName);
    if (loadedAnimatorController != null)
    {
        animator.runtimeAnimatorController = loadedAnimatorController;
    }
    else
    {
        Debug.LogWarning("Animator Controller not found: " + gameData.animatorControllerName);
    }
}

private void AddInventoryItems(GameData gameData)
{
    foreach (WeaponData weapon in gameData.weaponsInSlots)
    {
        playerInventory.Add(weapon);
    }

    foreach (PassiveData passive in gameData.passiveItemsInSlots)
    {
        playerInventory.Add(passive);
    }

    for (int i = 0; i < gameData.weaponsInSlots.Count; i++)
    {
        WeaponData weaponData = gameData.weaponsInSlots[i];
        int level = gameData.weaponLevels[i];
        Weapon weapon = playerInventory.Get(weaponData) as Weapon;
        if (weapon != null)
        {
            weapon.SetLevel(level);
        }
    }

    for (int i = 0; i < gameData.passiveItemsInSlots.Count; i++)
    {
        PassiveData passiveData = gameData.passiveItemsInSlots[i];
        int level = gameData.passiveLevels[i];
        Passive passive = playerInventory.Get(passiveData) as Passive;
        if (passive != null)
        {
            passive.SetLevel(level);
        }
    }
}


    public void LoadEnemiesAndWave(GameData gameData){
        foreach (EnemiesData enemyData in gameData.enemiesData)
    {
        GameObject enemyPrefab = Resources.Load<GameObject>(enemyData.enemyPrefabName);
        if (enemyPrefab != null)
        {
            GameObject enemyObject = Instantiate(enemyPrefab, enemyData.position, Quaternion.identity);
            EnemyStats enemyStats = enemyObject.GetComponent<EnemyStats>();
            enemyStats.currentHealth = enemyData.currentHealth;
            enemyStats.currentStats.moveSpeed = enemyData.currentMoveSpeed;
            enemyStats.currentStats.damage = enemyData.currentDamage;
        }
        Debug.Log(enemyData.enemyPrefabName);
    }
    }
   public void SaveGameData(ref GameData gameData)
{
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();
    gameData.playerPosition = transform.position;
    gameData.actualStat = actualStats;
    gameData.baseStat = baseStat;
    gameData.level = level;
    gameData.experience = experience;
    gameData.currentHealth = currentHeal;
    gameData.score = score;
    gameData.spriteRendererSpriteName = spriteRenderer.sprite.name;
    gameData.animatorControllerName = animator.runtimeAnimatorController.name;
    gameData.timeSurvival = gameManager.stopWatchTime;
    gameData.magnet = actualStats.magnet;
    gameData.weaponsInSlots.Clear();
    gameData.passiveItemsInSlots.Clear();
    gameData.weaponLevels.Clear();
    gameData.passiveLevels.Clear();
        foreach (PlayerInventory.Slot slot in playerInventory.weaponSlot)
    {
        if (slot.item != null && slot.item is Weapon weapon)
        {
            gameData.weaponsInSlots.Add((WeaponData)weapon.data);
            gameData.weaponLevels.Add(weapon.currentLevel); // Save the weapon level
            Debug.Log(weapon.currentStats.number);
        }
    }

    foreach (PlayerInventory.Slot slot in playerInventory.passiveSlot)
    {
        if (slot.item != null && slot.item is Passive passive)
        {
            gameData.passiveItemsInSlots.Add((PassiveData)passive.data);
            gameData.passiveLevels.Add(passive.currentLevel); // Save the passive level
        }
    }



    gameData.availableWeapons = new List<WeaponData>(playerInventory.availableWeapons);
    gameData.availablePassiveItems = new List<PassiveData>(playerInventory.availablePassive);
    EnemyStats enemyStats = new EnemyStats();
    enemyStats.SaveEnemies();
    gameData.enemiesData = new List<EnemiesData>(EnemyStats.enemiesData);
    SpawnManager.instance.SaveWaveData(ref gameData);
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

    public void IncreaseExperience(int amount)
    {
        int adjustedAmount = Mathf.CeilToInt(amount * actualStats.expMultiplier);
        Debug.Log("Exp: " + adjustedAmount);
        Debug.Log("ExpMultiple: " + cst.stats.expMultiplier);
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

            GameManager.instance.StartLevelUp();
            updateLevelDisplay();
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
//     private void OnTriggerEnter2D(Collider2D other)
// {
//     if (other.CompareTag("Enemy"))
//     {
//         PlayCollisionSound();
//     }
// }


// private void PlayCollisionSound()
// {
//     audioSource = GetComponent<AudioSource>();
//     hitSound = Resources.Load<AudioClip>("Audio/hurt");
//     // Play your collision sound here
//     if (audioSource != null)
//     {
//         audioSource.PlayOneShot(hitSound); // Replace 'yourCollisionClip' with your audio clip
//     }
// }

    public void updateHealBar()
    {
        healBar.fillAmount = currentHeal / actualStats.maxHeal;
    }

public void updateExpBar()
{
    // Ensure that experienceCap is not zero to avoid division by zero errors
    if (experienceCap > 0)
    {
        ExpBar.fillAmount = (float)experience / experienceCap;
    }
    else
    {
        ExpBar.fillAmount = 0f; // Or some other default value
    }
}


    public void updateLevelDisplay()
    {

        levelDisplay.text = "LV:" + level.ToString();
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
}
