using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.VFX;

public class Pickup : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float lifeSpan = 0.5f;
    protected PlayerStats target; // if the pickup has a target, then fly toward the target
    protected float speed; // tốc độ ba
    [Header("Exp and Heal")]
    public int Exp;
    public int Heal;
    public bool Coolected = false;
    EnemyStats enemyStats;
        public AudioClip hitSound; // Clip âm thanh va chạm
    private AudioSource audioSource; // Component AudioSource

    // Tạo ID khi đối tượng được khởi tạo
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Update(){
        if(target){
            // tính toán khoảng cách để di chuyển đến người chơi
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > 0.01f) 
{
    transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
}
else
{
    Destroy(gameObject);
}
        }
    }
    public virtual bool Collect(PlayerStats target, float speed, float lifeSpan = 0f){
        if(!this.target){
            this.target = target;
            this.speed = speed;
            if(lifeSpan > 0) this.lifeSpan = lifeSpan;
            PlayCollisionSound();
            Destroy(gameObject, Math.Max(0.01f,this.lifeSpan));
            return true;
        }
        return false;
    }
    private void PlayCollisionSound()
{
    audioSource = GetComponent<AudioSource>();
    if(Exp != 0) hitSound = Resources.Load<AudioClip>("Audio/getExp 1");
    if(Heal != 0) hitSound = Resources.Load<AudioClip>("Audio/heal 1");
    // Play your collision sound here
    if (audioSource != null)
    {
        audioSource.PlayOneShot(hitSound); // Replace 'yourCollisionClip' with your audio clip
    }
}
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Debug.Log("collect");
    //         Destroy(gameObject);
    //     }
    // }
    protected virtual void OnDestroy() {
        if(!target) return;
        if(Exp != 0 ) target.IncreaseExperience(Exp);
        if(Heal != 0 ) target.RestoreHeal(Exp);
    }
}
