using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObject/Weapon", order = 0)]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
    private GameObject prefabs;

    public GameObject Prefabs
    {
        get { return prefabs; }
        set { prefabs = value; }
    }

    // Base stat for weapon
    [SerializeField]
    float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    [SerializeField]
    float speed;
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration
    {
        get { return cooldownDuration; }
        set { cooldownDuration = value; }
    }
    [SerializeField]
    int pierce;
    public int Pierce
    {
        get { return pierce; }
        set { pierce = value; }
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

