using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector] public PlayerStats owner;
    [HideInInspector] public BOTStats botOwner;

    [HideInInspector] public Weapon weapon;
    public DamageBOP damageBOP;


    public float GetDamage(){
        return weapon.GetDamage();
    }
}
