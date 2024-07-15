using System;
using System.Resources;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string characterName;
    public CharacterData.Stats baseStat;
    public CharacterData.Stats actualStat;
    public int experience;
    public int level;
    public float[] position;
    public float currentHealth;
    public int score;
   public string spriteRendererSpriteName; // Store the sprite name
    public string animatorControllerName; // Store the animator controller name
    // public WeaponData startingWeaponName;
    public PlayerData(CharacterData characterData, PlayerStats playerStats)
    {
        characterName = characterData.Name;
        baseStat = characterData.stats;
        actualStat = playerStats.actualStats;
        experience = playerStats.experience;
        level = playerStats.level;
        position = new float[3];
        position[0] = playerStats.transform.position.x;
        position[1] = playerStats.transform.position.y;
        position[2] = playerStats.transform.position.z;
        currentHealth = playerStats.CurrentHeal;
        score = playerStats.score;
        // Store the name of the sprite
        spriteRendererSpriteName = playerStats.spriteRenderer.sprite.name;

        // Store the name of the animator controller
        animatorControllerName = playerStats.animator.runtimeAnimatorController.name;
    }
}
