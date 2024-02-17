using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "ScriptableObject/Items", order = 0)]
public class ItemScriptableObject : ScriptableObject
{
    public string _name;
    public GameObject prefabs;
}
