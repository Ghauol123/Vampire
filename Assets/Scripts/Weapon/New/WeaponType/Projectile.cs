using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource {projectile, owner}; // dame đến từ đâu từ vũ khí hay người chơi để tính được đường đẩy lùi enemy
    public DamageSource damageSource = DamageSource.projectile; // lấy ra projectile
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0,0,0);
    protected Rigidbody2D rb;
    protected int pierce; // số lần vũ khí chạm vào kẻ địch trước khi biến mất
    // Start is called before the first frame update
    private void Start() {
        Initialize();
    }
    public virtual void Initialize()
{
    rb = GetComponent<Rigidbody2D>();
    Weapon.Stats stats = weapon.GetStats();
    if(rb.bodyType == RigidbodyType2D.Kinematic){
        rb.angularVelocity = rotationSpeed.z;
        rb.velocity = transform.right * stats.speed;
    }
    float area = stats.area == 0 ? 1 : stats.area;
    transform.localScale = new Vector3(
        area * Math.Sign(transform.localScale.x),
        area * Math.Sign(transform.localScale.y),
        1
    );
    pierce = stats.piercing;
    if(stats.lifespan >0) Invoke("ReturnToPool", stats.lifespan);
    // if(hasAutoAim) AcquireAutoAimFacing();
}

protected virtual void ReturnToPool()
{
    CancelInvoke("ReturnToPool");
    ObjectPool.Instance.ReturnObject(gameObject);
}
    // public virtual void AcquireAutoAimFacing(){
    //     float aimAngle;
    //     EnemyStats[] targets = FindObjectsOfType<EnemyStats>();
    //     if(targets.Length > 0){
    //         EnemyStats selectedTarget = targets[UnityEngine.Random.Range(0,targets.Length)];
    //         Vector2 difference = selectedTarget.transform.position - transform.position;
    //         aimAngle = Mathf.Atan2(difference.y,difference.x) * Mathf.Rad2Deg;
    //     }
    //     else{
    //         aimAngle = UnityEngine.Random.Range(0f,360f);
    //     }
    //     transform.rotation = Quaternion.Euler(0,0,aimAngle);
    // }
    protected virtual void FixedUpdate() {
        rb = GetComponent<Rigidbody2D>();
                if (weapon == null) 
        {
            Debug.LogWarning("Weapon is null in Projectile FixedUpdate");
            return;
        }
        if(rb == null)
        {
            Debug.LogWarning("Rigidbody2D is null in Projectile FixedUpdate");
            return;
        }

        if(rb.bodyType == RigidbodyType2D.Kinematic){
            Weapon.Stats stats = weapon.GetStats();// lấy thông tin của vũ khí hiện tại
            if (stats == null)
            {
                Debug.LogWarning("Weapon stats are null in Projectile FixedUpdate");
                return;
            }
            transform.position += transform.right * stats.speed * Time.deltaTime; // vị trí của vũ khí 
            rb.MovePosition(transform.position); // di chuyển vũ khí 
            transform.Rotate(rotationSpeed*Time.deltaTime); // xoay vũ khí
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        EnemyStats es = other.GetComponent<EnemyStats>();
        if(es){
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
                        DamageBOP sourceDamage = owner != null ? DamageBOP.Player : DamageBOP.BOT;
            es.TakeDamage(GetDamage(), source, 5f, 0.2f, sourceDamage);
            
            Weapon.Stats stats = weapon.GetStats();
            pierce--;
            if(stats.hitEffect){
                Destroy(Instantiate(stats.hitEffect,transform.position,Quaternion.identity), 5f);
            }
        }
        if(pierce <= 0) ReturnToPool();
    }
}