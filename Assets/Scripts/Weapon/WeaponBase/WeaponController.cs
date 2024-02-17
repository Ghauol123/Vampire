using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject wst;
    public float currentCooldown;

    protected PlayerMoving playerMoving;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerMoving = FindObjectOfType<PlayerMoving>();
        currentCooldown = wst.CooldownDuration;
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
        currentCooldown = wst.CooldownDuration;
    }
}
