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
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
        currentDamage = wst.Damage;
        currentCooldownDuration = wst.CooldownDuration;
        currentPierce = wst.Pierce;
        currentSpeed = wst.Speed;
    }
        public float GetCurrrentDamage(){
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>(); // Sửa dòng này
            if (enemy != null)
            {
                enemy.TakeDamage(GetCurrrentDamage());
            }
        }
    }
}   
