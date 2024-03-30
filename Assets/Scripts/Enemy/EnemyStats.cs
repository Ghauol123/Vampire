using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject est;
    [HideInInspector]
    public float currentMoveSpeed;
    // [HideInInspector]
    public float CurrentHealts;
    [HideInInspector]
    public float currentDamage;
    float distanceDespawn = 20f;
    Transform playerMoving;
    PlayerStats PlayerStats;
    private void Awake() {
        currentDamage = est.Damage;
        CurrentHealts = est.maxhealt;
        currentMoveSpeed = est.MoveSpeed;
    }
    private void Start() {
        playerMoving = FindObjectOfType<PlayerMoving>().transform;
        PlayerStats = FindObjectOfType<PlayerStats>();
    }
    private void Update() {
        if(Vector2.Distance(transform.position,playerMoving.position) > distanceDespawn){
            OnDespawnEnemy();
        }
    }
    public void TakeDamage(float dmg){
        CurrentHealts -= dmg;
        if(CurrentHealts <= 0){
            Kill();
        }
    }
    public void Kill(){
        Destroy(gameObject);
        PlayerStats.PlusScore();
        // GameManager.instance.ScoreEndGame.text = "Score: "+PlayerStats.score.ToString();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            playerStats.TakeDamage(currentDamage);
        }
    }
    private void OnDestroy() {
        if(!gameObject.scene.isLoaded){
            return;
        }
        EnemySpawn enemySpawn = FindObjectOfType<EnemySpawn>();
        enemySpawn.OnEnemyKill();
    }
    public void OnDespawnEnemy(){
        EnemySpawn enemySpawn = FindObjectOfType<EnemySpawn>();
        transform.position = playerMoving.position + enemySpawn.spawningPosition[Random.Range(0,enemySpawn.spawningPosition.Count)].position;
    }
}
