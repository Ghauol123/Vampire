using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOTPickup : PlayerPickUp
{
    BOTStats bOTStats;

    // Start is called before the first frame update
    protected override void Start()
    {   
        bOTStats = GetComponentInParent<BOTStats>();
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Pickup p))
        {
            if (p.Collect(playerStats, pullSpeed))
            {
                Debug.Log("Bot collected pickup: " + p.name);
            }
            else
            {
                Debug.LogWarning("Failed to collect pickup for bot: " + p.name);
            }
        }
    }
}
