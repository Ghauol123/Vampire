using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickup : Pickup, ICollect
{
    public int Exp;
    private void Start()
    {
    }
    public void Collect()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.IncreaseExperience(Exp);
    }
}
