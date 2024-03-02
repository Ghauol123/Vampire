using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItems : MonoBehaviour
{
    [SerializeField]
    protected PlayerStats playerStats;
    public PassiveItemsScriptableObject passiveItemsScriptableObject;
    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        AppliedModifire();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void AppliedModifire(){

    }
}
