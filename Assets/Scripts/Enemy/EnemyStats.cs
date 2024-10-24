using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{

    [System.Serializable]
    public struct Resitances
    {
        [Range(0f, 1f)] public float freeze;
        [Range(0f, 1f)] public float kill;
        [Range(0f, 1f)] public float debuff;
    
        // public Resitances(float freeze, float kill, float debuff)
        // {
        //     this.freeze = freeze;
        //     this.kill = kill;
        //     this.debuff = debuff;
        // }
        public static Resitances operator *(Resitances a, float factor){
            a.freeze = Mathf.Min(1, a.freeze * factor);
            a.kill = Mathf.Min(1, a.kill * factor);
            a.debuff = Mathf.Min(1, a.debuff * factor);
            return a;
        }
    }
    [System.Serializable]
    public struct Stats{
        [Min(0)] public float maxHealth, moveSpeed, damage, knockbackMultiple;
        public Resitances resitances;
        [System.Flags]
        public enum Boostable {heal = 1, moveSpeed = 2, damage = 4, knockbackMultiple = 8, resitances = 16};
        public Boostable curseBoost, levelBoost;
        private static Stats Boots(Stats s1, float factor, Boostable boostable){
            if((boostable & Boostable.heal) != 0) s1.maxHealth *= factor;
            if((boostable & Boostable.moveSpeed) != 0) s1.moveSpeed *= factor;
            if((boostable & Boostable.damage) != 0) s1.damage *= factor;
            if((boostable & Boostable.knockbackMultiple) != 0) s1.knockbackMultiple /= factor;
            if((boostable & Boostable.resitances) != 0) s1.resitances *= factor;
            return s1;
        }
        //use the multiply operator to apply curse boosts
        public static Stats operator *(Stats s1, float factor){ return Boots(s1, factor, s1.curseBoost);}
        //use the power operator to apply level boosts
        public static Stats operator ^(Stats s1, float factor){ return Boots(s1, factor, s1.levelBoost);}
    }
    public Stats baseStats = new Stats{maxHealth = 100, moveSpeed = 0.5f, damage = 10, knockbackMultiple = 1, curseBoost =(Stats.Boostable)(1|2), levelBoost = 0};
    public Stats currentStats;
    public Stats Actual{
        get{return currentStats;}
    }
    public float currentHealth;
    // public float currentMoveSpeed;
    // public float CurrentHealts;
    // public float currentDamage;
    Transform playerMoving;
    PlayerStats PlayerStats;
    [Header("Damage Feedback")]
    public Color damageColor = new Color(0,0,0); // what the color of the damage flash should be
    public float damageFlashDuration = 0.2f; // How ong the flash should last
    public float deathFadeTime = 0.6f; // How much time it takes for the enemy to fade
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;
    Collider2D col;
        
    public AudioClip hitSound; // Clip âm thanh va chạm
    private AudioSource audioSource; // Component AudioSource

    public static int count;
    private void Awake() {
        count ++;
        // currentDamage = est.Damage;
        // CurrentHealts = est.maxhealt;
        // currentMoveSpeed = est.MoveSpeed;
    }
    private void Start() {
        RecalculateStats();
        currentHealth = currentStats.maxHealth;
        playerMoving = FindObjectOfType<PlayerMoving>().transform;
        PlayerStats = FindObjectOfType<PlayerStats>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        movement = GetComponent<EnemyMovement>();
        col =  GetComponent<Collider2D>();
    }
    public void RecalculateStats(){

        float curse = GameManager.GetCumulativeCurse(),
        level = GameManager.GetCumulativeLevels();
        currentStats = (baseStats *curse) ^ level;
        // currentStats = baseStats;

    }
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f){
        // currentHealth -= dmg;
        // Debug.Log(dmg);
        // StartCoroutine(DamageFlash());
        // if(knockbackForce > 0){
        //     Vector2 dir = (Vector2)transform.position - sourcePosition;
        //     // movement.KnockBack(dir.normalized * knockbackForce, knockbackDuration);
        // }
        // if(currentHealth <= 0){
        //     Kill();
        // }
        if(Mathf.Approximately(dmg, currentStats.damage)){
            if(UnityEngine.Random.value < currentStats.resitances.kill){
                return;
            }
        }
        currentHealth -= dmg;
        // Debug.Log(dmg);
        StartCoroutine(DamageFlash());
        if(currentHealth <= 0){
            Kill();
        }
    }
    
    IEnumerator DamageFlash(){
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }
    public void Kill()
    {
        col.enabled = false; 
        StartCoroutine(KillFade());
        PlayerStats.IncreaseKillnumber(1);

        // Drop items before returning to the pool
        DropRateManager dropManager = FindObjectOfType<DropRateManager>();
        if (dropManager != null) {
            dropManager.DropItems(transform.position);
        }
    }
    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origiAlpha = sr.color.a;
        while(t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origiAlpha);
        }
        PlayerStats.PlusScore();
        ObjectPool.Instance.ReturnObject(gameObject);

    }
    public void ResetEnemy()
{
    sr.color = originalColor; // Reset color
    col.enabled = true; // Enable collider
    currentHealth = currentStats.maxHealth; // Reset health
    // Reset any other necessary properties here
}
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            playerStats.TakeDamage(Actual.damage);
            PlayCollisionSound();
            // Debug.Log("Player Hit");
        }
    }
    private void PlayCollisionSound()
{
    audioSource = GetComponent<AudioSource>();
    hitSound = Resources.Load<AudioClip>("Audio/hurt");
    // Play your collision sound here
    if (audioSource != null)
    {
        audioSource.PlayOneShot(hitSound); // Replace 'yourCollisionClip' with your audio clip
    }
}
    // private void OnDestroy() {
    //     count --;
    // }
}
