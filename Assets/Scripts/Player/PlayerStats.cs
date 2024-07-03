using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;

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
    [Header("Visual")]
    public ParticleSystem dameEffect;
    public ParticleSystem blockEffect;
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
    public UnityEngine.UI.Image healBar;
    public UnityEngine.UI.Image ExpBar;
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
        // If the invicible timer is greater than 0, reduce the timer
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
        // cộng thông số từ item
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
    {// tăng kinh nghiệm
        experience += amount;
        levelCheckerUp();
        updateExpBar();
    }
    public void IncreaseHeal(float heal)
    {// hồi máu
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
        //Check if the player has enough experience to level up
        if (experience >= experienceCap)
        {
            //Level up the player and reduce their experience by the experience cap
            level++;
            experience -= experienceCap;
            int experienceCapIncrese = 0;
            //find the experience cap increase for the current level range
            // and increase the experience cap by that amount
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
        // nếu người chơi không bất tử thì nhận dame, giảm máu và đặt lại thời gian bất tử
        // nếu máu nhỏ hơn hoặc bằng 0 thì chết
        if (isInvicible == false)
        {
            // nếu có giáp thì giảm dame
            dmg -= actualStats.armor;
            // nếu dame lớn hơn 0 thì nhận dame
            if(dmg >0){
                CurrentHeal -= dmg;
                //nếu có hiệu ứng dame thì thực hiện
                // nếu máu nhỏ hơn hoặc bằng 0 thì chết
                if(dameEffect) Destroy(Instantiate(dameEffect,transform.position,Quaternion.identity),5f);
                invicibleTime = invicibleDuration;
                isInvicible = true;
                if (CurrentHeal <= 0)
                {
                    Kill();
                }
            }
            // nếu có hiệu ứng block thì thực hiện
            else{
                if(blockEffect) Destroy(Instantiate(dameEffect,transform.position,Quaternion.identity),5f);
            }
            updateHealBar();
        }
    }
    void updateHealBar()
    {
        // cập nhật thanh máu
        healBar.fillAmount = currentHeal / actualStats.maxHeal;
        // healDisplay.text = (currentHeal/cst.Maxheal).ToString();
    }
    void updateExpBar(){
        // cập nhật thanh kinh nghiệm
        ExpBar.fillAmount = (float)experience/experienceCap;
    }
    void updateLevelDisplay(){
        // cập nhật level
        levelDisplay.text = "LV:" + level.ToString();
    }
    public void RestoreHeal(float amount){
        // hồi máu
        if(CurrentHeal < actualStats.maxHeal){
            // chỉ khi mà máu người chơi nhỏ hơn máu tối đa mới hồi máu
            CurrentHeal += amount;
            if(CurrentHeal > actualStats.maxHeal){
                CurrentHeal = actualStats.maxHeal;
            }
        }
    }
    public void Recover(){
        // hồi máu theo thời gian
        if(CurrentHeal < actualStats.maxHeal){
            // CurrentHeal += CurrentHeal*Time.deltaTime;
            CurrentHeal += Stats.recovery*Time.deltaTime;
            // đảm bảo rằng máu của người chơi không vượt ngưỡng
            if(CurrentHeal > actualStats.maxHeal){
                CurrentHeal = actualStats.maxHeal;
            }
        }
        // cập nhật thanh máu
        updateHealBar();
    }
    public void Kill()
    {
        // nếu người chơi chết thì gọi hàm gameover
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
        // cộng điểm
        score += 10;
    }
    public PlayerStats(PlayerStats playerStats){
        playerInventory = playerStats.playerInventory;
        weaponIndex = playerStats.weaponIndex;
        passiveItemsIndex = playerStats.passiveItemsIndex;
        actualStats = playerStats.actualStats;
        float[] playerPosition = new float[3];
        playerPosition[0] = playerStats.transform.position.x;
        playerPosition[1] = playerStats.transform.position.y;
        playerPosition[2] = playerStats.transform.position.z;
    }
}
