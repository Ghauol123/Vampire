using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Melee : WeaponEffect
{
    public enum DamageSource {Melee, owner}; // dame đến từ đâu từ vũ khí hay người chơi để tính được đường đẩy lùi enemy
    // public enum DamageBOP {Player, BOT}
    public DamageSource damageSource = DamageSource.Melee; // lấy ra projectile
    // Start is called before the first frame update
    void Start()
    {
        Weapon.Stats stats = weapon.GetStats();
        float area = stats.area == 0 ? 1 : stats.area; // toán tử ba ngôi nếu area == 0 thì set = 1, ngược lại thì set = stats.area
        transform.localScale = new Vector3( // cập nhật kích thước trong cục bộ của vũ khí
            area * Math.Sign(transform.localScale.x), // cập nhập kích thước theo chiều x không thay đổi hướng
            area * Math.Sign(transform.localScale.y), // giống với x 
            1 // giá trị giữ nguyên trong không gian 3D
        );
        if(stats.lifespan >0) Destroy(gameObject,stats.lifespan);
    }

   // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        EnemyStats es = other.GetComponent<EnemyStats>();
        if(es){
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            // Determine damage source based on owner
            DamageBOP sourceDamage = owner != null ? DamageBOP.Player : DamageBOP.BOT;
                    Debug.Log($"Damage source: {sourceDamage}, Owner: {(owner != null ? "Player" : "BOT")}");

            es.TakeDamage(GetDamage(), source, 5f, 0.2f, sourceDamage);
            
            Weapon.Stats stats = weapon.GetStats();
            if(stats.hitEffect){
                Destroy(Instantiate(stats.hitEffect,transform.position,Quaternion.identity), 5f);
            }
        }
    }
}