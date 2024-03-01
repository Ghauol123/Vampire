using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemsScriptableObject", menuName = "ScriptableObject/PassiveItems", order = 0)]
public class PassiveItemsScriptableObject : ScriptableObject
{
    [SerializeField]
    float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    // Base stat for weapon
    [SerializeField]
    string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }
    [SerializeField]
    new string name;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    [SerializeField]
    string type;
    public string Type
    {
        get { return type; }
        set { type = value; }
    }
    [SerializeField]
    float multiple;
    public float Multiple
    {
        get { return multiple; }
        set { multiple = value; }
    }
    [SerializeField]
    int level;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    [SerializeField]
    GameObject nextLevelPrefabs;
    public GameObject NextLevelPrefabs
    {
        get { return nextLevelPrefabs; }
        set { nextLevelPrefabs = value; }
    }
    [SerializeField]
    Sprite icon;
    public Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }
}
