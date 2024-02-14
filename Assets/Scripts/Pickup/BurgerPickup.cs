using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerPickup : Pickup, ICollect
{
    public float healing;
    public void Collect()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.IncreaseHeal(healing);
    }
}
