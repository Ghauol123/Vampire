using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickup : Pickup, ICollect
{
    public int Exp;
    private void Start()
    {
    }
    public override void Collect()
    {
        if(hasBeenPickUp){
            return;
        }
        else {
            base.Collect();
        }
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.IncreaseExperience(Exp);
    }
}
