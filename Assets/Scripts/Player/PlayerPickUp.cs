using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Exp")){
            ExpPickup exp = other.GetComponent<ExpPickup>();
            exp.Collect();
        }
        if(other.CompareTag("Burger")){
            BurgerPickup burger = other.GetComponent<BurgerPickup>();
            burger.Collect();
        }
    }
    // private void OnTriggerEnter2D(Collider2D other) {
    //     if(other.gameObject.TryGetComponent(out ICollect Collectible)){
    //         Collectible.Collect();
    //     }
    // }
}
