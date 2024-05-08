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
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if(rb.bodyType == RigidbodyType2D.Dynamic){
             // bodytype ở đây là kiểm tra cơ thể của đối tượng, như Dynamic ở đây thì vũ khí có thể di chuyển và xoay theo vật lý. Còn có kinematic là điều khiển thông qua code và static là tĩnh
             // dòng này để quản lý được chuyển động của vũ khí như đạn hoặc là rìu
            rb.angularVelocity = rotationSpeed.z; //tốc độ quay của vũ khí
            rb.velocity = transform.right * stats.speed; //tốc độ di chuyển của vũ khí 
        }
        float area = stats.area == 0 ? 1 : stats.area; // toán tử ba ngôi nếu area == 0 thì set = 1, ngược lại thì set = stats.area
        transform.localScale = new Vector3( // cập nhật kích thước trong cục bộ của vũ khí
            area * Math.Sign(transform.localScale.x), // cập nhập kích thước theo chiều x không thay đổi hướng
            area * Math.Sign(transform.localScale.y), // giống với x 
            1 // giá trị giữ nguyên trong không gian 3D
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
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime; // vị trí của vũ khí 
            rb.MovePosition(transform.position); // di chuyển vũ khí 
            transform.Rotate(rotationSpeed*Time.fixedDeltaTime); // xoay vũ khí
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
