using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerPickUp : MonoBehaviour
{
    PlayerStats playerStats;
    CircleCollider2D playerCollector;
    public float pullSpeed;
    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        playerCollector.radius = playerStats.currentManget;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exp"))
        {
            ExpPickup exp = other.GetComponent<ExpPickup>();
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - other.transform.position).normalized;
            rb.AddForce(forceDirection*pullSpeed);
            exp.Collect();
        }
        if (other.CompareTag("Burger"))
        {
            BurgerPickup burger = other.GetComponent<BurgerPickup>();
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - other.transform.position).normalized;
            rb.AddForce(forceDirection*pullSpeed);
            burger.Collect();
        }
    }
    // private void OnTriggerEnter2D(Collider2D other) {
    //     if(other.gameObject.TryGetComponent(out ICollect Collectible)){
    //         Collectible.Collect();
    //     }
    // }
}
