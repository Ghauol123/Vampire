using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject est;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealts;
    [HideInInspector]
    public float currentDamage;
    float distanceDespawn = 20f;
    Transform playerMoving;
    private void Awake() {
        currentDamage = est.Damage;
        currentHealts = est.maxhealt;
        currentMoveSpeed = est.MoveSpeed;
    }
    private void Start() {
        playerMoving = FindObjectOfType<PlayerMoving>().transform;
    }
    private void Update() {
        if(Vector2.Distance(transform.position,playerMoving.position) > distanceDespawn){
            OnDespawnEnemy();
        }
    }
    public void TakeDamage(float dmg){
        currentHealts -= dmg;
        if(currentHealts <= 0){
            Kill();
        }
    }
    public void Kill(){
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            playerStats.TakeDamage(currentDamage);
        }
    }
    private void OnDestroy() {
        EnemySpawn enemySpawn = FindObjectOfType<EnemySpawn>();
        enemySpawn.OnEnemyKill();
    }
    public void OnDespawnEnemy(){
        EnemySpawn enemySpawn = FindObjectOfType<EnemySpawn>();
        transform.position = playerMoving.position + enemySpawn.spawningPosition[Random.Range(0,enemySpawn.spawningPosition.Count)].position;
    }
}
