using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerStats : MonoBehaviour
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

private void Awake()
{

    // Lấy tham chiếu đến GameManager đã có sẵn
    gameManager = FindObjectOfType<GameManager>();

    if (gameManager != null && !gameManager.isGameLoaded)
    {
        cst = CharacterSelected.GetData();
        // CharacterSelected.instance.DestroyInstance();

        playerInventory = GetComponent<PlayerInventory>();
        playerPickUp = GetComponentInChildren<PlayerPickUp>();
        baseStat = actualStats = cst.stats;
        playerPickUp.SetMagnet(actualStats.magnet);
        currentHeal = actualStats.maxHeal;
                    spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }
}

private void Start()
{

    if (gameManager != null && !gameManager.isGameLoaded)
    {
        spriteRenderer.sprite = cst.sprite;
        animator.runtimeAnimatorController = cst.animatorController;

        playerInventory.Add(cst.StartingWeapon);

        experienceCap = levelRanges[0].experienceCapIncrese;

        gameManager.AssignCharacter(cst);
        gameManager.Icon(cst);
    }
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
            public void SaveGame()
    {
        SaveSystem.SaveGame(cst, this);
    }

 public void LoadGame()
{
    PlayerData data = SaveSystem.LoadGame();
    if (data != null)
    {
        PlayerStats playerStats = this;
        playerStats.spriteRenderer = GetComponent<SpriteRenderer>();
        playerStats.animator = GetComponent<Animator>();
        // Load baseStat and actualStats separately
        baseStat = data.baseStat;
        actualStats = data.actualStat;

        experience = data.experience;
        level = data.level;
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        CurrentHeal = data.currentHealth;
        score = data.score;


        playerStats.spriteRenderer.sprite = Resources.Load<Sprite>(data.spriteRendererSpriteName);

        playerStats.animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(data.animatorControllerName);


        // Ensure to update experienceCap based on current level
        experienceCap = levelRanges[0].experienceCapIncrese;

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
            if (gameManager != null && !gameManager.isGameLoaded)
    {
        healBar.fillAmount = currentHeal / actualStats.maxHeal;}
    }

    public void updateExpBar()
    {
            if (gameManager != null && !gameManager.isGameLoaded)
    {
        ExpBar.fillAmount = (float)experience/experienceCap;}
    }

    public void updateLevelDisplay()
    {
            if (gameManager != null && !gameManager.isGameLoaded)
    {
        levelDisplay.text = "LV:" + level.ToString();}
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
