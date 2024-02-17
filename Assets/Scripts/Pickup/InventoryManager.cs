using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class ItemInfor{
        public string name;
    }
    public List<ItemInfor> inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<ItemInfor>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsItemInventory(string itemName)
    {
        return inventory.Any(item => item.name == itemName);
    }

    public void AddItemToInventory(string itemName)
    {
        ItemInfor newItem = new ItemInfor { name = itemName };
        inventory.Add(newItem);
    }
}
