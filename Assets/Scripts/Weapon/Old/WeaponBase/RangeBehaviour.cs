using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBehaviour : MonoBehaviour
{
    public WeaponScriptableObject wst;
    protected float currentDamage;
    protected float currentCooldownDuration;
    protected int currentPierce;
    protected float currentSpeed;
    protected Vector3 direction;

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
     public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if (dirx < 0 && diry == 0) //left
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if (dirx > 0 && diry > 0) //right up
        {
            rotation.z = 0f;
        }
        else if (dirx > 0 && diry < 0) //right down
        {
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry > 0) //left up
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry < 0) //left down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);    //Can't simply set the vector because cannot convert
    }
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>(); // Sửa dòng này
            if (enemy != null)
            {
                enemy.TakeDamage(GetCurrrentDamage(),transform.position);
            }
            Debug.Log("va chạm enemy");
        }
    }
}
