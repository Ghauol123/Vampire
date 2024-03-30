using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    InventoryManager inventoryManager;
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            OpenBox();
            Destroy(gameObject);
        }
    }
    public void OpenBox(){
        Debug.Log("Box open");
    }

}
