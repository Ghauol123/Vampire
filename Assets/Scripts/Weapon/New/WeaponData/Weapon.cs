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
        public Aura auraPrefabs;
        public Melee meleePrefabs;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;
        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstance;
        public static Stats operator +(Stats s1, Stats s2){
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.projectilePrefabs = s2.projectilePrefabs ?? s1.projectilePrefabs;
            result.auraPrefabs = s2.auraPrefabs ?? s1.auraPrefabs;
            result.meleePrefabs = s2.meleePrefabs ?? s1.meleePrefabs;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan =  s1.lifespan + s2.lifespan;
            result.damage =  s1.damage + s2.damage;
            result.damageVariance =  s1.damageVariance + s2.damageVariance;
            result.area =  s1.area + s2.area;
            result.speed =  s1.speed + s2.speed;
            result.cooldown =  s1.cooldown + s2.cooldown;
            result.number =  s1.number + s2.number;
            result.piercing =  s1.piercing + s2.piercing;
            result.projectileInterval =  s1.projectileInterval + s2.projectileInterval;
            result.knockback =  s1.knockback + s2.knockback;
            return result;
        }
        public float getDamage(){
            return damage + Random.Range(0,damageVariance);
        }
    }
    [SerializeField]public Stats currentStats;
    [HideInInspector]
    public ItemData data;
    protected float currentCooldown;
    protected PlayerMoving movement;
    // For dynamically created weapons, call initialise to set everything up
    public override void Initialise(ItemData data){
        base.Initialise(data);
        this.data = data;
        currentStats = ((WeaponData)data).baseStats;
        // Find the player stats
        movement =  GetComponentInParent<PlayerMoving>();
        // Set the cooldown to 0
        currentCooldown = currentStats.cooldown;
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
    public virtual float GetDamage(){
        // Might is a multiplier for the damage
        return currentStats.getDamage() *owner.Stats.might;
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
public virtual void SetLevel(int level)
{
    // Ensure the level is within valid bounds
    if (level < 1 || level > data.maxLevel)
    {
        Debug.LogWarning($"Level {level} is out of bounds for {data.name}.");
        return;
    }

    // Set the current level
    currentLevel = level;
    Debug.Log($"Set level for {data.name} to {currentLevel}. Current stats: Damage: {currentStats.damage}, Number: {currentStats.number}");
    // Check if the data is available
    if (data == null)
    {
        Debug.LogWarning("Weapon data is not initialized.");
        return;
    }
    // // Accumulate stats for each level up to the current level  
    //     for (int i = 2; i <= currentLevel; i++)
    // {
    //     currentStats += (Stats)data.GetLevelData(i);
    // }
    Stats currentStat = ((WeaponData)data).baseStats;
    for (int i = 2; i <= level; i++)
    {
        Weapon.Stats levelStats = (Stats)data.GetLevelData(i);
        Debug.Log($"Level {i} stats: Damage: {levelStats.damage}, Number: {levelStats.number}");
        // currentStat.damage += levelStats.damage;
        // currentStat.number += levelStats.number;
        // currentStat.damageVariance += levelStats.damageVariance;
        // currentStat.area += levelStats.area;
        // currentStat.speed += levelStats.speed;
        // currentStat.lifespan += levelStats.lifespan;
        // currentStat.number += levelStats.number;
        // currentStat.piercing += levelStats.piercing;
        // currentStat.projectileInterval += levelStats.projectileInterval;
        // currentStat.knockback += levelStats.knockback;
        // currentStat.cooldown += levelStats.cooldown;
        currentStat += levelStats;
    }
    // Optionally reinitialize cooldown if necessary
    currentCooldown = currentStats.cooldown;
    Debug.Log($"Set level for {data.name} to {currentLevel}. Current stats: Damage: {currentStat.damage}, Number: {currentStat.number}");
}
}
