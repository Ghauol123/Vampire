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
            if (GameManager.instance != null)
            {
                GameManager.instance.CurrentHealDisplay.text = string.Format("Health: {0} / {1}",currentHeal, actualStats.maxHeal);
            }
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
    [HideInInspector]
    public PlayerInventory playerInventory;
    public int weaponIndex;
    public int passiveItemsIndex;

    [Header("UI")]
    public UnityEngine.UI.Image healBar;
    public UnityEngine.UI.Image ExpBar;
    public TMP_Text levelDisplay;
    public TMP_Text healDisplay;
    PlayerData playerData;
    GameManager gameManager;
    DataPersistenceManager dataPersistenceManager;

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


    public  void InitializeNewGame()
    {
        // Khởi tạo cho trò chơi mới
        cst = CharacterSelected.GetData();
        if (CharacterSelected.instance != null && CharacterSelected.instance.gameObject.activeInHierarchy)
        {
            CharacterSelected.instance.DestroyInstance();
        }

        playerInventory = GetComponent<PlayerInventory>();
        playerPickUp = GetComponentInChildren<PlayerPickUp>();
        baseStat = actualStats = cst.stats;
        playerPickUp.SetMagnet(actualStats.magnet);
        currentHeal = actualStats.maxHeal;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Thiết lập sprite và animator cho người chơi
        spriteRenderer.sprite = cst.sprite;
        animator.runtimeAnimatorController = cst.animatorController;
        playerInventory.availableWeapons.Insert(0, cst.StartingWeapon);
        // Thêm vũ khí khởi đầu vào kho người chơi
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
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();
    transform.position = gameData.playerPosition;
    baseStat = gameData.baseStat;
    actualStats = gameData.actualStat;
    level = gameData.level;
    experience = gameData.experience;
    currentHeal = gameData.currentHealth;
    score = gameData.score;
    gameManager.stopWatchTime = gameData.timeSurvival;

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

    playerInventory = GetComponent<PlayerInventory>();
    playerInventory.ClearInventory();
    playerInventory.availableWeapons = new List<WeaponData>(gameData.availableWeapons);
    playerInventory.availablePassive = new List<PassiveData>(gameData.availablePassiveItems);
    
    foreach (WeaponData weapon in gameData.weaponsInSlots)
    {
        playerInventory.Add(weapon);
    }
    foreach (PassiveData passive in gameData.passiveItemsInSlots)
    {
        playerInventory.Add(passive);
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

    gameData.weaponsInSlots.Clear();
    foreach (PlayerInventory.Slot slot in playerInventory.weaponSlot)
    {
        if (slot.item != null && slot.item is Weapon weapon)
        {
            gameData.weaponsInSlots.Add((WeaponData)weapon.data);
        }
    }

    gameData.passiveItemsInSlots.Clear();
    foreach (PlayerInventory.Slot slot in playerInventory.passiveSlot)
    {
        if (slot.item != null && slot.item is Passive passive)
        {
            gameData.passiveItemsInSlots.Add((PassiveData)passive.data);
        }
    }

    gameData.availableWeapons = new List<WeaponData>(playerInventory.availableWeapons);
    gameData.availablePassiveItems = new List<PassiveData>(playerInventory.availablePassive);
}

    public void SaveGame()
    {
        SaveSystem.SaveGame(cst, this);
    }

    public void LoadGame()
    {
        PlayerData data = SaveSystem.LoadGame();
        if (data != null)
        {
            // Khôi phục các thuộc tính của PlayerStats
            baseStat = data.baseStat;
            actualStats = data.actualStat;
            experience = data.experience;
            level = data.level;
            transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            currentHeal = data.currentHealth;
            score = data.score;

            // Cập nhật sprite và animator
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>(data.spriteRendererSpriteName);
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(data.animatorControllerName);

            // Đảm bảo cập nhật experienceCap dựa trên cấp độ hiện tại
            experienceCap = levelRanges[0].experienceCapIncrese;

            // Khôi phục kho đồ và các thành phần khác nếu cần
            playerInventory = GetComponent<PlayerInventory>();
            playerPickUp = GetComponentInChildren<PlayerPickUp>();
            playerPickUp.SetMagnet(actualStats.magnet);

            GameManager.instance.AssignCharacter(cst);
            GameManager.instance.Icon(cst);
        }
        else
        {
            Debug.LogError("Failed to load game data.");
        }
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
        experience += amount;
        levelCheckerUp();
        updateExpBar();
    }

    public void IncreaseHeal(float heal)
    {
        if (CurrentHeal < cst.stats.maxHeal)
        {
            CurrentHeal += heal;
            if (CurrentHeal > cst.stats.maxHeal)
            {
                CurrentHeal = cst.stats.maxHeal;
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
            if(experience >= experienceCap) levelCheckerUp();
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
        healBar.fillAmount = currentHeal / actualStats.maxHeal;
    }

    public void updateExpBar()
    {

        ExpBar.fillAmount = (float)experience/experienceCap;
    }

    public void updateLevelDisplay()
    {

        levelDisplay.text = "LV:" + level.ToString();
    }

    public void RestoreHeal(float amount)
    {
        if(CurrentHeal < actualStats.maxHeal)
        {
            CurrentHeal += amount;
            if(CurrentHeal > actualStats.maxHeal)
            {
                CurrentHeal = actualStats.maxHeal;
            }
        }
    }

    public void Recover()
    {
        if(CurrentHeal < actualStats.maxHeal)
        {
            CurrentHeal += Stats.recovery*Time.deltaTime;
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
