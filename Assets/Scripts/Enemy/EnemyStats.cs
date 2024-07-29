using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject est;
    public float currentMoveSpeed;
    public float CurrentHealts;
    public float currentDamage;
    Transform playerMoving;
    PlayerStats PlayerStats;
    [Header("Damage Feedback")]
    public Color damageColor = new Color(0,0,0); // what the color of the damage flash should be
    public float damageFlashDuration = 0.2f; // How ong the flash should last
    public float deathFadeTime = 0.6f; // How much time it takes for the enemy to fade
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;
    public static int count;
    private void Awake() {
        count ++;
        currentDamage = est.Damage;
        CurrentHealts = est.maxhealt;
        currentMoveSpeed = est.MoveSpeed;
    }
    private void Start() {
        playerMoving = FindObjectOfType<PlayerMoving>().transform;
        PlayerStats = FindObjectOfType<PlayerStats>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        movement = GetComponent<EnemyMovement>();
    }
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f){
        CurrentHealts -= dmg;
        StartCoroutine(DamageFlash());
        if(knockbackForce > 0){
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            // movement.KnockBack(dir.normalized * knockbackForce, knockbackDuration);
        }
        if(CurrentHealts <= 0){
            Kill();
        }
    }
    IEnumerator DamageFlash(){
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }
    public void Kill(){
        StartCoroutine(KillFade());
        // GameManager.instance.ScoreEndGame.text = "Score: "+PlayerStats.score.ToString();
    }
    IEnumerator KillFade(){
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origiAlpha = sr.color.a;
        while(t<deathFadeTime){
            yield return w;
            t += Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1-t/deathFadeTime)*origiAlpha);
        }
        Destroy(gameObject);
        PlayerStats.PlusScore();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            playerStats.TakeDamage(currentDamage);
            Debug.Log("Player Hit");
        }
    }
    private void OnDestroy() {
        count --;
    }
}
