using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class PlayerStats : MonoBehaviour
{
    public CharacterData cst;
    public CharacterData.Stats baseStat;
    [SerializeField] CharacterData.Stats actualStats;
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
        // if we try and set the current heal, the UI interface
        // on the pause scree will also be updated
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
    // public float Maxheal{
    //     get {return actualStats.maxHeal;}
    //      set
    //     {
    //         if(actualStats.maxHeal != value){
    //         actualStats.maxHeal = value;
    //         if (GameManager.instance != null)
    //         {
    //             GameManager.instance.CurrentHealDisplay.text = string.Format("Health: {0} / {1}",currentHeal, actualStats.maxHeal);
    //         }
    //         }
    //         //Update the real time value of the stat
    //         //Add any additional logic here that needs to be excuted when the value changes
    //     }
    // }
    // public float CurrentRecovery{
    //     get{return recoveryStats;}
    //     set{recoveryStats = value;}
    // }
    // public float recoveryStats
    // {
    //     get { return actualStats.recovery; }
    //     set
    //     {
    //         if(actualStats.recovery != value){
    //         actualStats.recovery = value;
    //         if (GameManager.instance != null)
    //         {
    //             GameManager.instance.CurrentrecoveryDisplay.text = "Recovery:" + actualStats.recovery;
    //         }
    //         }
    //     }
    // }
    
    // public float CurrentSpeed
    // {
    //     get { return MoveSpeed; }
    //     set{MoveSpeed = value;}
    // }
    //   public float MoveSpeed
    // {
    //     get { return actualStats.moveSpeed; }
    //     set
    //     {
    //         if(actualStats.moveSpeed != value){
    //         actualStats.moveSpeed = value;
    //         if (GameManager.instance != null)
    //         {
    //             GameManager.instance.CurrentSpeedDisplay.text = "MoveSpeed:" + actualStats.moveSpeed;
    //         }
    //         }
    //     }
    // }
    // public float CurrentMight
    // {
    //     get { return Might; }
    //     set{Might = value;}
    // }
    //   public float Might
    // {
    //     get { return actualStats.might; }
    //     set
    //     {
    //         if(actualStats.might != value){
    //         actualStats.might = value;
    //         if (GameManager.instance != null)
    //         {
    //             GameManager.instance.CurrentMightDisplay.text = "Might:" + actualStats.might;
    //         }
    //         }
    //     }
    // }
    //  public float CurrentProjectileSpeed
    // {
    //     get { return ProjectileSpeed; }
    //     set{ProjectileSpeed = value;}
    // }
    //   public float ProjectileSpeed
    // {
    //     get { return actualStats.speed; }
    //     set
    //     {
    //         if(actualStats.speed != value){
    //         actualStats.speed = value;
    //         if (GameManager.instance != null)
    //         {
    //             GameManager.instance.CurrentProjectTileSpeedDisplay.text = "Projectile Speed:" + actualStats.speed;
    //         }
    //         }
    //     }
    // }
    //    public float CurrentManget
    // {
    //     get { return Magnet; }
    //     set{Magnet = value;}
    // }
    //   public float Magnet
    // {
    //     get { return actualStats.magnet; }
    //     set
    //     {
    //         if(actualStats.magnet != value){
    //         actualStats.magnet = value;
    //         if (GameManager.instance != null)
    //         {
    //             GameManager.instance.CurrentMagnetDisplay.text = "Magnet:" + actualStats.magnet;
    //         }
    //         }
    //     }
    // }
    #endregion
    // public List<GameObject> startWeapon;
    public ParticleSystem dameEffect;
    //Experience and level of the player
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
    // Class for defining a level range and the corresponding experience cap increse for that range
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


    PlayerInventory playerInventory;
    public int weaponIndex;
    public int passiveItemsIndex;
    

    [Header("UI")]
    public Image healBar;
    public Image ExpBar;
    public TMP_Text levelDisplay;
    public TMP_Text healDisplay;
     private void Awake()
    {
        cst = CharacterSelected.GetData();
        CharacterSelected.instance.DestroyInstance();

        playerInventory = GetComponent<PlayerInventory>();
        playerPickUp = GetComponentInChildren<PlayerPickUp>();
        baseStat = actualStats = cst.stats;
        playerPickUp.SetMagnet(actualStats.magnet);
        currentHeal = actualStats.maxHeal;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // SpawnWeapon(cst.);
    }
    private void Start()
    {

        spriteRenderer.sprite = cst.sprite;
        animator.runtimeAnimatorController = cst.animatorController;
        // CurrentHeal = cst.stats.maxHeal;
        // Might = cst.stats.might;
        // ProjectileSpeed = cst.stats.speed;
        // recoveryStats = cst.stats.recovery;
        // MoveSpeed = cst.stats.moveSpeed;
        // Magnet = cst.stats.magnet;

        playerInventory.Add(cst.StartingWeapon);


        experienceCap = levelRanges[0].experienceCapIncrese;
        // đặt max kinh nghiệp đầu tiên là max kinh nghiệm ở level 0
        // playerAnimation = GetComponent<Animator>();
        // playerAnimation.runtimeAnimatorController = cst._Animation;
        // spr = GetComponent<SpriteRenderer>();
        // spr.sprite = cst.Image;
        // GameManager.instance.CurrentHealDisplay.text = "Heal:" + currentHeal;
        // GameManager.instance.CurrentrecoveryDisplay.text = "Recovery:" + CurrentRecovery;
        // GameManager.instance.CurrentSpeedDisplay.text = "Speed:" + CurrentSpeed;
        // GameManager.instance.CurrentMightDisplay.text = "Might:" + CurrentMight;
        // GameManager.instance.CurrentProjectTileSpeedDisplay.text = "ProjectileSpeed:" + CurrentProjectileSpeed;
        // GameManager.instance.CurrentMagnetDisplay.text = "Magnet:" + CurrentManget;
        
        GameManager.instance.AssignCharacter(cst);
        GameManager.instance.Icon(cst);
    }
    private void Update()
    {
        if (invicibleTime > 0)
        {
            invicibleTime -= Time.deltaTime;

        }
        // If the invicible timer has reacher 0, set the invicibility flag to false
        else if (isInvicible)
        {
            isInvicible = false;
        }
        Recover();
        updateLevelDisplay();
    }
    public void RecalculatedStats(){
        actualStats = baseStat;
        foreach(PlayerInventory.Slot s in playerInventory.passiveSlot){
            Passive p = s.item as Passive;
            if(p){
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
            //Level up the player and reduce their experience by the experience cap
            level++;
            experience -= experienceCap;
            int experienceCapIncrese = 0;
            //find the experience cap increase for the current level range
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
        // nếu người chơi không bất tử thì nhận dame, giảm máu và đặt lại thời gian bất tử
        if (isInvicible == false)
        {
            CurrentHeal -= dmg;
            //nếu có hiệu ứng dame thì thực hiện
            if(dameEffect) Destroy(Instantiate(dameEffect,transform.position,Quaternion.identity),5f);
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
        healBar.fillAmount = currentHeal / actualStats.maxHeal;
        // healDisplay.text = (currentHeal/cst.Maxheal).ToString();
    }
    void updateExpBar(){
        ExpBar.fillAmount = (float)experience/experienceCap;
    }
    void updateLevelDisplay(){
        levelDisplay.text = "LV:" + level.ToString();
    }
    public void RestoreHeal(float amount){
        if(CurrentHeal < actualStats.maxHeal){
            // chỉ khi mà máu người chơi nhỏ hơn máu tối đa mới hồi máu
            CurrentHeal += amount;
            if(CurrentHeal > actualStats.maxHeal){
                CurrentHeal = actualStats.maxHeal;
            }
        }
    }
    public void Recover(){
        if(CurrentHeal < actualStats.maxHeal){
            // CurrentHeal += CurrentHeal*Time.deltaTime;
            CurrentHeal += Stats.recovery*Time.deltaTime;
            // đảm bảo rằng máu của người chơi không vượt ngưỡng
            if(CurrentHeal > actualStats.maxHeal){
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
            // GameManager.instance.AssignLevelReachedUI(level);

            GameManager.instance.AssignScore(score);
            GameManager.instance.AssignWeaponAndPassiveItem(playerInventory.weaponSlot, playerInventory.passiveSlot);
            GameManager.instance.GameOver();
        }
    }
    public void PlusScore()
    {
        score += 10;
    }
    public void SpawnWeapon(GameObject weapon)
    {
        // if (weaponIndex > inventoryManager.weaponSlot.Count - 1)// trừ 1 vì bắt đầu từ 0;
        // {
        //     Debug.Log("Full weapon");
        //     return;
        // }
        // GameObject spawnWeapon = Instantiate(weapon, transform.position, Quaternion.identity);

        // spawnWeapon.transform.SetParent(transform);
        // // startWeapon.Add(spawnWeapon);
        // inventoryManager.AddWeapon(weaponIndex, spawnWeapon.GetComponent<WeaponController>()); // thêm vũ khí vào inventory

        // weaponIndex++;
    }
    public void SpawnPassiveItems(GameObject passiveItems)
    {
        // if (passiveItemsIndex > inventoryManager.passiveItemsSlot.Count - 1)// trừ 1 vì bắt đầu từ 0;
        // {
        //     Debug.Log("Full weapon");
        //     return;
        // }
        // GameObject spawnPassiveItems = Instantiate(passiveItems, transform.position, Quaternion.identity);
        // spawnPassiveItems.transform.SetParent(transform);
        // // startWeapon.Add(spawnWeapon);
        // inventoryManager.AddPassiveItem(passiveItemsIndex, spawnPassiveItems.GetComponent<PassiveItems>()); // thêm vũ khí vào inventory

        // passiveItemsIndex++;
    }

}
