using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        // public static Stats operator +(Stats s1, Stats s2){
        //     Stats result = new Stats();
        //     result.name = s2.name ?? s1.name;
        //     result.description = s2.description ?? s1.description;
        //     result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
        //     result.projectilePrefabs = s2.projectilePrefabs ?? s1.projectilePrefabs;
        //     result.auraPrefabs = s2.auraPrefabs ?? s1.auraPrefabs;
        //     result.meleePrefabs = s2.meleePrefabs ?? s1.meleePrefabs;
        //     result.spawnVariance = s2.spawnVariance;
        //     result.lifespan =  s1.lifespan + s2.lifespan;
        //     result.damage =  s1.damage + s2.damage;
        //     result.damageVariance =  s1.damageVariance + s2.damageVariance;
        //     result.area =  s1.area + s2.area;
        //     result.speed =  s1.speed + s2.speed;
        //     result.cooldown =  s1.cooldown + s2.cooldown;
        //     result.number =  s1.number + s2.number;
        //     result.piercing =  s1.piercing + s2.piercing;
        //     result.projectileInterval =  s1.projectileInterval + s2.projectileInterval;
        //     result.knockback =  s1.knockback + s2.knockback;
        //     return result;
        // }
        public static Stats operator +(Stats s1, Stats s2)
{
    Stats result = new Stats
    {
        name = s2.name ?? s1.name,
        Icon = s2.Icon ?? s1.Icon,
        description = s2.description ?? s1.description,
        hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect,
        projectilePrefabs = s2.projectilePrefabs ?? s1.projectilePrefabs,
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
    // For dynamically created weapons, call initialise to set everything up
    public virtual void Initialise(WeaponData data){
        base.Initialise(data);
        this.data = data;
        currentStats =data.baseStats;
        // Find the player stats
        movement =  FindAnyObjectByType<PlayerMoving>();
        // Set the cooldown to 0
        currentCooldown = currentStats.cooldown;
        owner = FindAnyObjectByType<PlayerStats>();

    }
    protected virtual void Awake() {
        if(data) currentStats = ((WeaponData)data).baseStats;
    }
    protected virtual void Start(){
        if(data){
            Initialise(data);
        }
    }
    protected virtual void Update(){
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0){
            Attack(currentStats.number);
        }
    }
    public override bool DoLevelUp(){
        base.DoLevelUp();
        if(!CanLevelUp()){
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}. max leve of {2} already reached", name,currentLevel, data.maxLevel));
            return false;
        }
        // otherwise, add stats of the next level to our weapon
        currentStats += (Stats)data.GetLevelData(++currentLevel);
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
        
        // Lấy thông số chí mạng từ nhân vật
        float criticalChance = owner.Stats.criticalChance;
        float criticalMultiplier = owner.Stats.criticalMultiplier;

        // Kiểm tra xem có chí mạng hay không
        bool isCriticalHit = Random.value < criticalChance;

        if (isCriticalHit)
        {
            baseDamage *= criticalMultiplier; // Nhân sát thương với hệ số chí mạng
            Debug.Log("Critical Hit!");
        }

        return baseDamage * owner.Stats.might; // Áp dụng sát thương với chỉ số might của nhân vật
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
    //     public void LoadWeaponStatsByLevel(int level)
    // {
    //     if (currentWeaponData != null)
    //     {
    //         currentStats = (Stats)currentWeaponData.GetLevelData(level);
    //     }
    // }
    public virtual void SetLevel(int level)
    {
        if (level < 1 || level > data.maxLevel)
        {
            Debug.LogWarning($"Level {level} is out of bounds for {data.name}.");
            return;
        }
        currentLevel = level;
        currentStats = ((WeaponData)data).baseStats;
        for (int i = 2; i <= level; i++)
        {
            currentStats += (Stats)data.GetLevelData(i);
        }
        Debug.Log($"Set level for {data.name} to {currentLevel}. Current stats: {currentStats.damage}");
    }
}