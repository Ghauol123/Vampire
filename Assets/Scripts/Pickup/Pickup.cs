using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float lifeSpan = 0.5f;
    protected PlayerStats target; // if the pickup has a target, then fly toward the target
    protected float speed; // tốc độ ba
    [Header("Exp and Heal")]
    public int Exp;
    public int Heal;
    protected virtual void Update(){
        if(target){
            // tính toán khoảng cách để di chuyển đến người chơi
            Vector2 distance = target.transform.position - transform.position;
            if(distance.sqrMagnitude > speed*speed*Time.deltaTime){
                // cho biết khoảng cách và hướng của đường đi
                transform.position += (Vector3)distance.normalized*speed*Time.deltaTime;
            }
            else{
                Destroy(gameObject);
            }
        }
    }
    public virtual bool Collect(PlayerStats target, float speed, float lifeSpan = 0f){
        if(!this.target){
            this.target = target;
            this.speed = speed;
            if(lifeSpan > 0) this.lifeSpan = lifeSpan;
            Destroy(gameObject, Math.Max(0.01f,this.lifeSpan));
            return true;
        }
        return false;
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
