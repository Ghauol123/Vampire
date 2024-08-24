using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Projectile
{
    private Animator animator; // Tham chiếu đến Animator
    private bool hasExploded = false; // Đảm bảo bom chỉ nổ một lần

    // Hàm này được gọi khi script bắt đầu hoạt động
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>(); // Lấy tham chiếu đến Animator của đối tượng bom
    }

    // Hàm này được gọi khi một đối tượng khác va chạm với collider của bom
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Enemy không
        if (collision.CompareTag("Enemy") && !hasExploded)
        {
            hasExploded = true; // Đánh dấu rằng bom đã nổ
            animator.SetBool("isExploding", true); // Chuyển đổi sang animation nổ
            StartCoroutine(DestroyAfterExplosion());
        }
    }

    // Coroutine để chờ animation nổ kết thúc trước khi phá hủy đối tượng bom
    private IEnumerator DestroyAfterExplosion()
    {
        // Chờ animation nổ kết thúc (dựa trên thời gian của animation)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Phá hủy đối tượng bom sau khi nổ
        Destroy(gameObject);
    }
}
