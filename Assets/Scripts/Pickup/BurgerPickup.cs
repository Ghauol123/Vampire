using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerPickup : Pickup, ICollect
{
    public float healing;
    public override void Collect()
    {
        if (hasBeenPickUp)
        {
            return;
        }
        else
        {
            base.Collect();
        }
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.IncreaseHeal(healing);
    }
}
