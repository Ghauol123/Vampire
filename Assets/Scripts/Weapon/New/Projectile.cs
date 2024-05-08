using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource {projectile, owner};
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0,0,0);
    protected Rigidbody2D rb;
    protected int pierce;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if(rb.bodyType == RigidbodyType2D.Dynamic){
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
        if(stats.lifespan >0) Destroy(gameObject,stats.lifespan);
        if(hasAutoAim) AcquireAutoAimFacing();
    }
    public virtual void AcquireAutoAimFacing(){
        float aimAngle;
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();
        if(targets.Length > 0){
            EnemyStats selectedTarget = targets[UnityEngine.Random.Range(0,targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y,difference.x) * Mathf.Rad2Deg;
        }
        else{
            aimAngle = UnityEngine.Random.Range(0f,360f);
        }
        transform.rotation = Quaternion.Euler(0,0,aimAngle);
    }
    protected virtual void FixedUpdate() {
        if(rb.bodyType == RigidbodyType2D.Kinematic){
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed*Time.fixedDeltaTime);
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
            es.TakeDamage(GetDamage(), source);
            Weapon.Stats stats = weapon.GetStats();
            pierce--;
            if(stats.hitEffect){
                Destroy(Instantiate(stats.hitEffect,transform.position,Quaternion.identity), 5f);
            }
        }
        if(pierce <= 0) Destroy(gameObject);
    }
}
