// using System.Collections.Generic;
// using static PlayerStats;

// [System.Serializable]
// public class PlayerData
// {
//     // public CharacterData characterData;
//     public CharacterData.Stats baseStat;
//     public CharacterData.Stats actualStat;
//     public float[] position;
//     public int experience;
//     public int level;
//     public float currentHealth;
//     public int score;
//     public string spriteRendererSpriteName;
//     public string animatorControllerName;
//     public List<levelRange> levelRanges;

//     // public List<PlayerInventory.Slot> inventory;

//     public PlayerData(CharacterData cst, PlayerStats playerStats)
//     {
//         // characterData = cst;
//         baseStat = playerStats.baseStat;
//         actualStat = playerStats.actualStats;
//         position = new float[3];
//         position[0] = playerStats.transform.position.x;
//         position[1] = playerStats.transform.position.y;
//         position[2] = playerStats.transform.position.z;
//         experience = playerStats.experience;
//         level = playerStats.level;
//         currentHealth = playerStats.CurrentHeal;
//         score = playerStats.score;
//         spriteRendererSpriteName = playerStats.spriteRenderer.sprite.name;
//         animatorControllerName = playerStats.animator.runtimeAnimatorController.name;
//         levelRanges = playerStats.levelRanges;
//         // inventory = playerStats.playerInventory.GetInventory();
//     }
// }
public class PlayerData{
        public int Score { get; set; }
        public string CharacterName { get; set; }
        public string PlayerName { get; set; }
        public int Level { get; set; }  // Thêm level vào thông tin người chơi

}