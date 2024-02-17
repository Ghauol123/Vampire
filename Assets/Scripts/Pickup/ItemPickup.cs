using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    InventoryManager inventoryManager;
    public string _name;
    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (inventoryManager.IsItemInventory(name))
            {
                Debug.LogWarning("item is already in inventory");
                return;
            }
            else
            {
                inventoryManager.AddItemToInventory(name);
                Debug.LogWarning("Item added to bag");
                Destroy(gameObject);
            }
        }
    }
}
