using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class Weapon : Item
{
    [System.Serializable]
    public class Stats : LevelData{
        public string type;
        [Header("Visuals")]
        public Projectile projectilePrefabs;
        public Bomb bombPrefabs;
        public Aura auraPrefabs;
        public Melee meleePrefabs;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;
        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstance;
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats
            {
                name = s2.name ?? s1.name,
                Icon = s2.Icon ?? s1.Icon,
                description = s2.description ?? s1.description,
                hitEffect = s2.hitEffect ?? s1.hitEffect,
                projectilePrefabs = s2.projectilePrefabs ?? s1.projectilePrefabs,
                bombPrefabs = s2.bombPrefabs ?? s1.bombPrefabs,
                auraPrefabs = s2.auraPrefabs ?? s1.auraPrefabs,
                meleePrefabs = s2.meleePrefabs ?? s1.meleePrefabs,
                spawnVariance = s2.spawnVariance,
                lifespan = s1.lifespan + s2.lifespan,
                damage = s1.damage + s2.damage,
                damageVariance = s1.damageVariance + s2.damageVariance,
                area = s1.area + s2.area,
                speed = s1.speed + s2.speed,
                cooldown = s1.cooldown + s2.cooldown,
                number = s1.number + s2.number,
                piercing = s1.piercing + s2.piercing,
                projectileInterval = s1.projectileInterval + s2.projectileInterval,
                knockback = s1.knockback + s2.knockback
            };
            return result;
        }

        public float getDamage(){
            return damage + Random.Range(0,damageVariance);
        }
        public Weapon.Stats currentStat;
    }
    [SerializeField]public Weapon.Stats currentStats;
    [HideInInspector]
    public ItemData data;
    protected float currentCooldown;
    protected PlayerMoving movement;
    protected Transform ownerTransform = null;
    protected float lastDirectionX;
        public DamageBOP damageBOP;


    // For dynamically created weapons, call initialise to set everything up
    public virtual void Initialise(WeaponData data){
        base.Initialise(data);
        this.data = data;
        currentStats =data.baseStats;
        // Find the player stats
        movement =  FindAnyObjectByType<PlayerMoving>();
        // Set the cooldown to 0
        currentCooldown = currentStats.cooldown;
    }
//     public void AssignOwnerBasedOnParent()
// {
//     // owner = null;
//     // bOTowner = null;
//     if (transform.parent != null)
//     {
//         // Check if the parent is a bot
//         var botStats = transform.parent.GetComponent<BOTStats>();
//         if (botStats != null)
//         {
//             bOTowner = botStats; // Assign botOwner
//             owner = null; // Ensure owner is null
//             return;
//         }

//         // Check if the parent is a player
//         var playerStats = transform.parent.GetComponent<PlayerStats>();
//         if (playerStats != null)
//         {
//             owner = playerStats; // Assign owner
//             bOTowner = null; // Ensure botOwner is null
//         }
//     }
// }
    protected virtual void Awake() {
        if(data) currentStats = ((WeaponData)data).baseStats;
        owner = FindAnyObjectByType<PlayerStats>();
        bOTowner = FindAnyObjectByType<BOTStats>();

    }
    protected override void Start(){
        if(data){
            Initialise(data);
        }
        // AssignOwnerBasedOnParent();
        base.Start();
        if(owner == null && bOTowner != null){
            ownerTransform = bOTowner.transform;
        }
        else{
            ownerTransform = owner.transform;
        }
    }
    protected virtual void Update(){
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0){
            Attack(currentStats.number);
        }

        // Determine the direction based on the actual owner
        if (owner != null)
        {
            // If the owner is a player
            PlayerMoving playerMove = owner.GetComponent<PlayerMoving>();
            if (playerMove != null && playerMove.lastMovedVector.x != 0)
            {
                lastDirectionX = playerMove.lastMovedVector.x;
            }
        }
        else if (bOTowner != null)
        {
            // If the owner is a bot
            BotMoving botMove = bOTowner.GetComponent<BotMoving>();
            if (botMove != null && botMove.lastMovedVector.x != 0)
            {
                lastDirectionX = botMove.lastMovedVector.x;
            }
        }
    }
    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        if(!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}. max level of {2} already reached", name, currentLevel, data.maxLevel));
            return false;
        }

        // Lưu trữ prefabs hiện tại
        Projectile currentProjectilePrefab = currentStats.projectilePrefabs;
        Bomb currentBombPrefab = currentStats.bombPrefabs;
        Aura currentAuraPrefab = currentStats.auraPrefabs;
        Melee currentMeleePrefab = currentStats.meleePrefabs;

        // Nâng cấp stats
        currentStats += (Stats)data.GetLevelData(++currentLevel);

        // Khôi phục prefabs nếu chúng bị null sau khi nâng cấp
        if (currentStats.projectilePrefabs == null) currentStats.projectilePrefabs = currentProjectilePrefab;
        if (currentStats.bombPrefabs == null) currentStats.bombPrefabs = currentBombPrefab;
        if (currentStats.auraPrefabs == null) currentStats.auraPrefabs = currentAuraPrefab;
        if (currentStats.meleePrefabs == null) currentStats.meleePrefabs = currentMeleePrefab;

        Debug.Log($"Leveled up {name} to level {currentLevel}. Current bomb prefab: {currentStats.bombPrefabs}");

        return true;
    }
    public virtual bool canAttack(){
        return currentCooldown <= 0f;
    }
    protected virtual bool Attack(int attackCount = 1){
        if(canAttack()){
            currentCooldown += currentStats.cooldown;
            return true;
        }
        
        return false;
    }
    // public virtual float GetDamage(){
    //     // Might is a multiplier for the damage
    //     return currentStats.getDamage() *owner.Stats.might;
    // }
public virtual float GetDamage()
{
    float baseDamage = currentStats.getDamage();

    if (owner != null) // If the owner is a player
    {
        float criticalChance = owner.Stats.criticalChance;
        float criticalMultiplier = owner.Stats.criticalMultiplier;
        float might = owner.Stats.might;

        // Determine if the hit is critical
        bool isCriticalHit = Random.value < criticalChance;

        if (isCriticalHit)
        {
            baseDamage *= criticalMultiplier; // Apply critical multiplier
            Debug.Log("Player Critical Hit!");
        }
        baseDamage *= might; // Apply might multiplier
    }
    else if (bOTowner != null) // If the owner is a bot
    {
        float botCriticalChance = bOTowner.Stats.criticalChance;
        float botCriticalMultiplier = bOTowner.Stats.criticalMultiplier;
        float botMight = bOTowner.Stats.might;

        // Determine if the hit is critical
        bool isBotCriticalHit = Random.value < botCriticalChance;

        if (isBotCriticalHit)
        {
            baseDamage *= botCriticalMultiplier; // Apply critical multiplier
            Debug.Log("Bot Critical Hit!");
        }
        baseDamage *= botMight; // Apply might multiplier
    }

    return baseDamage;
}

    public virtual Stats GetStats(){return currentStats;}

    public virtual bool ActivateCooldown(bool strict = false){
        // If we are in strict mode, we can only activate cooldown if it is already 0
        if(strict && currentCooldown > 0) return false;
        // Otherwise, we can always activate cooldown
        float actualCooldown = currentStats.cooldown *owner.Stats.cooldown;
        // Cooldown is reduced by the cooldown reduction stat
        currentCooldown = Mathf.Min(actualCooldown,currentCooldown*actualCooldown);
        return true;
    }
    // public virtual void SetLevel(int level)
    // {
    //     if (level < 1 || level > data.maxLevel)
    //     {
    //         Debug.LogWarning($"Level {level} is out of bounds for {data.name}.");
    //         return;
    //     }
    //     currentLevel = level;
    //     currentStats = ((WeaponData)data).baseStats;
    //     for (int i = 2; i <= level; i++)
    //     {
    //         currentStats += (Stats)data.GetLevelData(i);
    //     }
    //     Debug.Log($"Set level for {data.name} to {currentLevel}. Current stats: {currentStats.damage}");
    // }
}
