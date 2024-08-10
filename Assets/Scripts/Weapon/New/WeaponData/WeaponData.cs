using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/WeaponData", order = 0)]
public class WeaponData : ItemData
{
    [HideInInspector] public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;

public override Item.LevelData GetLevelData(int level)
{
    if (level < 1)
    {
        Debug.LogWarning($"Invalid level {level}. Returning base stats.");
        return baseStats;
    }

    // Handle level 1 separately to ensure no index out of range
    if (level == 1)
    {
        return baseStats;
    }

    // Adjust the index for linearGrowth
    int linearGrowthIndex = level - 2;
    if (linearGrowthIndex >= 0 && linearGrowthIndex < linearGrowth.Length)
    {
        return linearGrowth[linearGrowthIndex];
    }
    // Handle randomGrowth if linearGrowth does not cover the level
    if (randomGrowth.Length > 0)
    {
        return randomGrowth[Random.Range(0, randomGrowth.Length)];
    }

    // Fallback if no level data is configured
    Debug.LogWarning($"Weapon doesn't have its level up stats configured for level {level}!");
    return new Weapon.Stats();
}

}
