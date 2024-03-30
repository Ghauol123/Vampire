using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : MonoBehaviour
{
    public WeaponScriptableObject wst;
    protected float currentDamage;
    protected float currentCooldownDuration;
    protected int currentPierce;
    protected float currentSpeed;

    public float destroyAfterSeconds;
    // Start is called before the first frame update
      private void Awake()
    {
        currentDamage = wst.Damage;
        currentSpeed = wst.Speed;
        currentCooldownDuration = wst.CooldownDuration;
        currentPierce = wst.Pierce;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
    public float GetCurrrentDamage(){
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>(); // Sửa dòng này
            enemy.TakeDamage(GetCurrrentDamage());
            Debug.Log("va chạm enemy");
        }
    }
}   
