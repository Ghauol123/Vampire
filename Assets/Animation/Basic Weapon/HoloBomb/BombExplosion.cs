using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : WeaponEffect
{
    private Animator anim;
    public float explosionDuration = 1f; // Thời gian vụ nổ tồn tại

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(HandleExplosion());
    }

    // Xử lý vụ nổ kèm animation
    IEnumerator HandleExplosion()
    {
        anim.SetTrigger("Explode"); // Kích hoạt animation vụ nổ
        yield return new WaitForSeconds(explosionDuration); // Chờ animation kết thúc
        Destroy(gameObject); // Hủy đối tượng sau khi vụ nổ kết thúc
    }
}
