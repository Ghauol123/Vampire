using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StatData", menuName = "StatData", order = 0)]

public class StatsData : ItemData
{
    public Stats.Modifier baseStats;
    public Stats.Modifier[] growth;
public override Item.LevelData GetLevelData(int level)
{
    if (level < 1)
    {
        Debug.LogWarning($"Invalid level {level}. Returning base stats.");
        return baseStats;
    }

    if (level == 1)
    {
        // If the level is 1, return the base stats
        return baseStats;
    }

    int growthIndex = level - 2;
    if (growthIndex >= 0 && growthIndex < growth.Length)
    {
        return growth[growthIndex];
    }

    Debug.LogWarning($"Passive doesn't have its level up stats configured for level {level}!");
    return new Stats.Modifier();
}
}
