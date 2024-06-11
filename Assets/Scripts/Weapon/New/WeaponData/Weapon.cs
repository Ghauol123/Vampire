using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats{
        public string name, description,type;
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
        public Sprite Icon;
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
    protected Stats currentStats;
    public WeaponData data;
    protected float currentCooldown;
    protected PlayerMoving movement;
    // For dynamically created weapons, call initialise to set everything up
    public virtual void Initialise(WeaponData data){
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement =  GetComponentInParent<PlayerMoving>();
        currentCooldown = currentStats.cooldown;
    }
    protected virtual void Awake() {
        if(data) currentStats = data.baseStats;
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
        currentStats += data.GetLevelData(++currentLevel);
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
        return currentStats.getDamage() *owner.Stats.might;
    }
    public virtual Stats GetStats(){return currentStats;}
}
