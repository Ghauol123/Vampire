using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject prefabs;
    public float damage;
    public float speed;
    public float cooldownDuration;
    private float currentCooldown;
    public int pierce;
    protected PlayerMoving playerMoving;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerMoving = FindObjectOfType<PlayerMoving>();
        currentCooldown = cooldownDuration;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0)
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        currentCooldown = cooldownDuration;
    }
}
