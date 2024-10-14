// using System;
// using System.Collections;
// using UnityEngine;

// public class Bomb : WeaponEffect
// {
//     public enum DamageSource {projectile, owner}; // dame đến từ đâu từ vũ khí hay người chơi để tính được đường đẩy lùi enemy
//     public DamageSource damageSource = DamageSource.projectile; // lấy ra projectile
//     public bool hasAutoAim = false;
//     public Vector3 rotationSpeed = new Vector3(0,0,0);
//     protected Rigidbody2D rb;
//     Animator animator;
//     protected int pierce; // số lần vũ khí chạm vào kẻ địch trước khi biến mất
//     // Start is called before the first frame update
//     private void Start() {
//         Initialize();
//         animator = GetComponent<Animator>();
//     }
//     public virtual void Initialize()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         Weapon.Stats stats = weapon.GetStats();
//         if(rb.bodyType == RigidbodyType2D.Dynamic){
//             rb.angularVelocity = rotationSpeed.z;
//             rb.velocity = transform.right * stats.speed;
//         }
//         float area = stats.area == 0 ? 1 : stats.area;
//         transform.localScale = new Vector3(
//             area * Math.Sign(transform.localScale.x),
//             area * Math.Sign(transform.localScale.y),
//             1
//         );
//         pierce = stats.piercing;
//         if(stats.lifespan >0) Invoke("ReturnToPool", stats.lifespan);
//     }

//     // protected virtual void ReturnToPool()
//     // {
//     //     CancelInvoke("ReturnToPool");
//     //     ObjectPool.Instance.ReturnObject(gameObject);
//     // }
//     protected virtual void FixedUpdate() {
//         rb = GetComponent<Rigidbody2D>();
//                 if (weapon == null) 
//         {
//             Debug.LogWarning("Weapon is null in Projectile FixedUpdate");
//             return;
//         }
//         if(rb == null)
//         {
//             Debug.LogWarning("Rigidbody2D is null in Projectile FixedUpdate");
//             return;
//         }

//         if(rb.bodyType == RigidbodyType2D.Kinematic){
//             Weapon.Stats stats = weapon.GetStats();// lấy thông tin của vũ khí hiện tại
//             if (stats == null)
//             {
//                 Debug.LogWarning("Weapon stats are null in Projectile FixedUpdate");
//                 return;
//             }
//             transform.position += transform.right * stats.speed * Time.deltaTime; // vị trí của vũ khí 
//             rb.MovePosition(transform.position); // di chuyển vũ khí 
//             transform.Rotate(rotationSpeed*Time.deltaTime); // xoay vũ khí
//         }
        
//     }
//     protected void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Enemy"))
//         {
//             EnemyStats enemyStats = other.GetComponent<EnemyStats>();
//             if (enemyStats)
//             {
//                 Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
//                 enemyStats.TakeDamage(GetDamage(), source);
                
//                 // Trigger explosion animation
//                 if (animator != null)
//                 {
//                     // animator.SetTrigger("Explode");
//                     // // Disable the collider to prevent multiple hits
//                     // GetComponent<Collider2D>().enabled = false;
//                     // // Return to pool after animation finishes
//                     // float explosionDuration = animator.GetCurrentAnimatorStateInfo(0).length;
//                     // Invoke("ReturnToPool", explosionDuration);
//                       animator.SetBool("isExploding", true);
//                         StartCoroutine(HandleExplosion());
//                 }
//                 else
//                 {
//                     ReturnToPool();
//                 }
                
//                 Debug.Log("Bomb hit enemy at: " + transform.position);
//             }
//         }
//         else
//         {
//             Debug.Log("Bomb hit non-enemy object at: " + transform.position);
//             // Optionally, you might want to explode on any collision
//             // Uncomment the next line if you want the bomb to explode on any collision
//             // ReturnToPool();
//         }
//     }

//     protected virtual void ReturnToPool()
//     {
//         // Ensure everything is reset before returning to pool
//         CancelInvoke();
//         if (animator != null)
//         {
//             animator.ResetTrigger("Explode");
//         }
//         ObjectPool.Instance.ReturnObject(gameObject);
//         Debug.Log("Bomb returned to pool");
//     }
//         private IEnumerator HandleExplosion()
//     {
//         // Chờ trong thời gian nổ
//         yield return new WaitForSeconds(2f);
        
//         // Hủy bom sau khi nổ
//         Destroy(gameObject);
//     }
// }


using System.Collections;
using UnityEngine;

public class Bomb : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);
    protected Rigidbody2D rb;
    Animator animator;
    protected int pierce;

    public float explosionRadius = 2f; // Bán kính vụ nổ
    public LayerMask enemyLayerMask;   // Lớp đối tượng enemy để xác định mục tiêu khi nổ

    private void Start()
    {
        Initialize();
        animator = GetComponent<Animator>();
    }

    public virtual void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }
        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y),
            1
        );
        pierce = stats.piercing;
        if (stats.lifespan > 0) Invoke("ReturnToPool", stats.lifespan);
    }

    protected virtual void FixedUpdate()
    {
        if (weapon == null)
        {
            Debug.LogWarning("Weapon is null in Projectile FixedUpdate");
            return;
        }
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody2D is null in Projectile FixedUpdate");
            return;
        }

        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            if (stats == null)
            {
                Debug.LogWarning("Weapon stats are null in Projectile FixedUpdate");
                return;
            }
            transform.position += transform.right * stats.speed * Time.deltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TriggerExplosion();
            Debug.Log("Bomb hit enemy at: " + transform.position);
        }
        else
        {
            Debug.Log("Bomb hit non-enemy object at: " + transform.position);
        }
    }

    private void TriggerExplosion()
    {
        if (animator != null)
        {
            animator.SetBool("isExploding", true);
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(HandleExplosion());

            // Gây sát thương cho tất cả enemy trong bán kính vụ nổ
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);
            foreach (Collider2D enemy in enemiesHit)
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
                    enemyStats.TakeDamage(GetDamage(), source);
                }
            }
        }
        else
        {
            ReturnToPool();
        }
    }

    private IEnumerator HandleExplosion()
    {
        // Lấy độ dài của animation nổ
        float explosionDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        // Chờ trong thời gian nổ hoàn tất
        yield return new WaitForSeconds(explosionDuration);

        // Trả bom về pool sau khi nổ
        ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        CancelInvoke();
        if (animator != null)
        {
            animator.SetBool("isExploding", false); // Reset trạng thái nổ
        }
        ObjectPool.Instance.ReturnObject(gameObject);
            GetComponent<Collider2D>().enabled = true;

        Debug.Log("Bomb returned to pool");
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ bán kính vụ nổ khi chọn đối tượng trong Unity Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
