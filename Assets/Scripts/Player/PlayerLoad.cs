// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerLoad : MonoBehaviour
// {
//     PlayerStats playerStats;
//     public SpriteRenderer spriteRenderer;
//     public Animator animator;
//      CharacterData.Stats baseStat;
//     CharacterData.Stats actualStats;
//     private void Awake() {
//         playerStats = FindAnyObjectByType<PlayerStats>();
//     }
//     private void Start() {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//         animator = GetComponent<Animator>();
//     }
//         public void SaveGame()
//     {
//         SaveSystem.SaveGame(playerStats.cst, playerStats);
//     }

//  public void LoadGame()
// {
//     PlayerData data = SaveSystem.LoadGame();
//     if (data != null)
//     {
//         // Load baseStat and actualStats separately
//         baseStat = data.baseStat;
//         actualStats = data.actualStat;

//         playerStats.experience = data.experience;
//         playerStats.level = data.level;
//         transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
//         playerStats.CurrentHeal = data.currentHealth;
//         playerStats.score = data.score;


//         playerStats.spriteRenderer.sprite = Resources.Load<Sprite>(data.spriteRendererSpriteName);

//         playerStats.animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(data.animatorControllerName);

//         // Ensure to update experienceCap based on current level
//         playerStats.experienceCap = playerStats.levelRanges[0].experienceCapIncrese;

//         GameManager.instance.AssignCharacter(playerStats.cst);
//         GameManager.instance.Icon(playerStats.cst);
//     }
//     else
//     {
//         Debug.LogError("Failed to load game data.");
//     }
// }
// }
