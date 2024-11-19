using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;
    #endregion

    #region Game State
    public GameState currentState;
    public GameState previousState;
    public bool isGameLoaded;
    public bool IsGamePause = false;
    public bool IsLevelUp = false;
    public bool IsGameOver => currentState == GameState.GameOver;
    public bool chosingUpgrade => currentState == GameState.LevelUp;
    #endregion

    #region UI References
    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject SinglePlayerResultScreen;
    public GameObject BotModeResultScreen;
    public GameObject gameOverScreen;
    public GameObject LevelUpScreen;
    public int stackLevelups = 0;

    [Header("Single Player Result Elements")]
    public TMP_Text kill;
    public TMP_Text levelReach;
    public TMP_Text ScoreEndGame;
    public TMP_Text timeSurvival;
    public TMP_Text stopWacthDisplay;
    public Image characterImage;
    public TMP_Text characterName;
    public Image IconCharacter;
    public List<Image> weaponIcon;
    public List<Image> passiveItemIcon;

    [Header("Bot Mode Result Elements")]
    public TMP_Text timeSurvivalBotMode;
 
    public TMP_Text playerScore;
    public TMP_Text playerKills;
    public TMP_Text botKills;
    public TMP_Text playerLevel;
    public Image playerCharacterImage;
    public Image botCharacterImage;
    public TMP_Text playerCharacterName;
    public TMP_Text botCharacterName;
    public List<Image> botWeaponIcon;
    public List<Image> botPassiveItemIcon;
     public List<Image> PlayerweaponIcon;
    public List<Image> PlayerpassiveItemIcon;
    #endregion

    #region Game Systems
    [Header("Time")]
    public float TimeLimit;
    public float stopWatchTime;

    [Header("Damage Text")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20f;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;
    #endregion

    #region Component References
    private PlayerStats[] playerStats;
    private BOTStats bOTStats;
    private FirebaseSaveGame firebaseSaveGame;
    private PlayerInventory playerInventory;
    private BOTInventory bOTInventory;
    #endregion
    #region Bot System
    [Header("Bot System")]
    public GameObject botPrefab;  // Kéo prefab BOT vào đây trong Inspector
    private GameObject currentBot;  // Để theo dõi bot đã spawn
    public GameMode playMode;
    #endregion
    public static int GetCumulativeLevels(){
        if(instance == null) return 1;
        int totalLevel = 0;
        foreach (var playerStat in instance.playerStats)
        {
            totalLevel += playerStat.level;
        }
        return Mathf.Max(1, totalLevel);
    }
    public static float GetCumulativeCurse(){
        if(!instance) return 1;
        float totalCurse = 0;
        foreach (var playerStat in instance.playerStats){
            totalCurse += playerStat.Stats.curse;
            Debug.Log(totalCurse);
        }
        return Mathf.Max(1, 1 + totalCurse);
    }
    void Start()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("have a gameManager");
            Destroy(gameObject);
        }
        LevelUpScreen.SetActive(false);
        playerStats = FindObjectsOfType<PlayerStats>();
        firebaseSaveGame = FindObjectOfType<FirebaseSaveGame>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        bOTInventory = FindAnyObjectByType<BOTInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found!");
        }
        playMode = CharacterSelected.instance.gamemode;  // Lấy gamemode từ PlayerPrefs
        SpawnBot();
    }
    void SpawnBot(){
        if(playMode == GameMode.SinglePlayer){
            return;
        }
        else if(playMode == GameMode.BotMode){
                if (botPrefab == null)
    {
        Debug.LogError("Bot prefab is not assigned!");
        return;
    }

    if (currentBot != null)
    {
        Debug.LogWarning("Bot already exists!");
        return;
    }
    currentBot = Instantiate(botPrefab, new Vector3(1,1,0), Quaternion.identity);
            bOTStats = currentBot.GetComponent<BOTStats>();
        bOTInventory = currentBot.GetComponent<BOTInventory>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.GamePlay:
                TestChangeState();
                UpdateStopWatch();
                IsGamePause = false;
                break;
            case GameState.Paused:
                TestChangeState();
                IsGamePause = true;
                break;
            case GameState.GameOver:
            case GameState.LevelUp:
                break;
        }
    }
    public void ChangeState(GameState newState)
    {
        if (playerInventory != null && playerInventory.CheckFullLevelAndSlots())
        {
            Debug.Log("Inventory is full and all items are at max level.");
            // Có thể thêm xử lý đặc biệt ở đây nếu cần
        }

        previousState = currentState;
        currentState = newState;
        Debug.Log($"Game state changed to: {newState}");
    }
    public void PauseGame()
    // Pause the game
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Game is Paused");
        }
    }
    public void ResumeGame()
    // Resume the game
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("Game Resume");
        }
    }
    public void TestChangeState()
    // Test if the game state should be changed
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void DisableScreen()
    // Disable all the screen
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        LevelUpScreen.SetActive(false);
        SinglePlayerResultScreen.SetActive(false);
        BotModeResultScreen.SetActive(false);
    }
public async void GameOver()
{
    stopWacthDisplay.enabled = false;
    timeSurvival.text = stopWacthDisplay.text;
    ChangeState(GameState.GameOver);
    Time.timeScale = 0f;
    gameOverScreen.SetActive(true);
    DisplayResultScreen();
    
    if (playMode == GameMode.SinglePlayer && playerInventory != null)
    {
        AssignWeaponAndPassiveItem(playerInventory.weaponSlot, playerInventory.passiveSlot);
    }
    else if (playMode == GameMode.BotMode)
    {
            if (playerInventory != null)
            {
                AssignPPlayerWeaponAndPassiveItem(playerInventory.weaponSlot, playerInventory.passiveSlot); // Gán cho người chơi
            }
            
            if (bOTInventory != null)
            {
                AssignBotWeaponAndPassiveItem(bOTInventory.weaponSlot, bOTInventory.passiveSlot); // Gán cho bot
            }
    }
    
    if (playerStats != null && playerStats.Length > 0)
    {
        kill.text = playerStats[0].killnumber.ToString();
    }
    TimeSpan timeSpan = TimeSpan.FromSeconds(stopWatchTime);
    string mapName = SceneManager.GetActiveScene().name;

    // Lấy userId và username của current user
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    string playerName = "Unknown";
    
    await FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .Child(userId)
        .Child("userInfo")
        .Child("username")
        .GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Value != null)
            {
                playerName = task.Result.Value.ToString();
            }
        });

    // Xử lý coin
    int firebaseCoin = await FirebaseLoadCoin.instance.GetCurrentCoinFromFirebase();
    int gameCoin = 0;
    foreach (var playerStat in playerStats)
    {
        gameCoin += playerStat.coin;
    }
    int totalCoin = firebaseCoin + gameCoin;
    await FirebaseLoadCoin.instance.UpdateCoinInFirebase(totalCoin);

    // Tạo PlayerData cho người chơi chính
    PlayerData playerData = null;
    PlayerData botData = null;

    // Kiểm tra chế độ chơi
    if (playMode == GameMode.SinglePlayer)
    {
        // Chế độ chơi đơn, chỉ lấy thông tin player
        foreach (var playerStat in playerStats)
        {
            playerData = new PlayerData
            {
                Score = playerStat.score,
                CharacterName = playerStat.cst.Name,
                PlayerName = playerName,
                Level = playerStat.level
            };
            break; // Chỉ lấy thông tin player đầu tiên vì chơi đơn
        }
    }
    else if (playMode == GameMode.BotMode)
    {
                    if (bOTStats.GetComponent<BOTStats>() != null)
            {
                // Nếu là BOT
                botData = new PlayerData
                {
                    Score = bOTStats.score,
                    CharacterName = bOTStats.cst.Name,
                    PlayerName = "Bot",
                };
            }
        // Chế độ chơi với bot, lấy thông tin cả player và bot
        foreach (var playerStat in playerStats)
        {
                // Nếu là Player
                playerData = new PlayerData
                {
                    Score = playerStat.score,
                    CharacterName = playerStat.cst.Name,
                    PlayerName = playerName,
                    Level = playerStat.level
                };
        }
    }

    // Kiểm tra và lưu điểm
    if (playerData != null)
    {
        Debug.Log($"Saving game data - Mode: {playMode}, Player: {playerData.PlayerName}, Score: {playerData.Score}");
        firebaseSaveGame.SaveScoreToFirebase(
            playerData,
            botData, // Sẽ là null trong chế độ SinglePlayer
            timeSpan,
            mapName,
            playMode
        );
    }
    else
    {
        Debug.LogError("No player data found for saving score!");
    }
}

    private int CalculateFinalScore()
    {
        int totalScore = 0;
        
        // Kiểm tra null
        if (playerStats == null || playerStats.Length == 0)
        {
            Debug.LogWarning("No player stats found!");
            return 0;
        }

        // Tính tổng điểm từ tất cả playerStats
        foreach (var playerStat in playerStats)
        {
            if (playerStat != null)
            {
                totalScore += playerStat.score;
                Debug.Log($"Adding score from player: {playerStat.score}, Total: {totalScore}");
            }
        }

        Debug.Log($"Final Score calculated: {totalScore}");
        return totalScore;
    }
    private void CalculateTotalKills()
{
    int playerKills = 0;
    int botKills = 0;

    if (playerStats == null || playerStats.Length == 0)
    {
        Debug.LogWarning("No stats found!");
        return;
    }

    foreach (var stat in playerStats)
    {
        if (stat != null)
        {
            if (stat.GetComponent<BOTStats>() != null)
            {
                // Nếu là BOT
                botKills = stat.killnumber;
                Debug.Log($"BOT kills: {stat.killnumber}, Total BOT kills: {botKills}");
            }
            else
            {
                // Nếu là Player
                playerKills = stat.killnumber;
                Debug.Log($"Player kills: {stat.killnumber}, Total Player kills: {playerKills}");
            }
        }
    }

    Debug.Log($"Final Kill Count - Player: {playerKills}, BOT: {botKills}");
}
    public void DisplayResultScreen()
    {
        if (playMode == GameMode.SinglePlayer)
        {
            SinglePlayerResultScreen.SetActive(true);
            BotModeResultScreen.SetActive(false);
        }
        else if (playMode == GameMode.BotMode)
        {
            SinglePlayerResultScreen.SetActive(false);
            BotModeResultScreen.SetActive(true);
            
            // Update Bot Mode Result Screen
            foreach (var stat in playerStats)
            {
                playerScore.text = stat.score.ToString();
                timeSurvivalBotMode.text = stopWacthDisplay.text;
                playerKills.text = stat.killnumber.ToString();
                playerLevel.text = stat.level.ToString();
                playerCharacterImage.sprite = stat.cst.Icon;
                playerCharacterName.text = stat.cst.Name;
            }

            if (bOTStats != null)
            {
                // botScore.text = bOTStats.score.ToString();
                botKills.text = bOTStats.killnumber.ToString();
                // botLevel.text = bOTStats.level.ToString();
                botCharacterImage.sprite = bOTStats.cst.Icon;
                botCharacterName.text = bOTStats.cst.Name;
            }
        }
    }
    public void AssignCharacter(CharacterData cst)
    // Assign the character data to the result screen
    {
        characterImage.sprite = cst.Icon;
        characterName.text = cst.Name;
    }
    public void Icon(CharacterData cst)
    {
        IconCharacter.sprite = cst.Icon;
    }
    public void AssignLevel(int levelCharacter)
    {
        levelReach.text = levelCharacter.ToString();
    }
    public void AssignScore(int score)
    {
        ScoreEndGame.text = score.ToString();
    }
    public void AssignWeaponAndPassiveItem(List<PlayerInventory.Slot> weapon, List<PlayerInventory.Slot> passiveItems)
    {
        Debug.Log($"Assigning weapons and passives. Weapons: {weapon.Count}, Weapon Icons: {weaponIcon.Count}");
        
        // Assign weapons
        for (int i = 0; i < weaponIcon.Count; i++)
        {
            if (i < weapon.Count && weapon[i].item != null)
            {
                weaponIcon[i].sprite = weapon[i].image.sprite;
                weaponIcon[i].enabled = true;
                Debug.Log($"Assigned weapon {i}: {weapon[i].item.name}");
            }
            else
            {
                weaponIcon[i].enabled = false;
                Debug.Log($"Disabled weapon slot {i}");
            }
        }

        // Assign passive items
        for (int i = 0; i < passiveItemIcon.Count; i++)
        {
            if (i < passiveItems.Count && passiveItems[i].item != null)
            {
                passiveItemIcon[i].sprite = passiveItems[i].image.sprite;
                passiveItemIcon[i].enabled = true;
                Debug.Log($"Assigned passive {i}: {passiveItems[i].item.name}");
            }
            else
            {
                passiveItemIcon[i].enabled = false;
                Debug.Log($"Disabled passive slot {i}");
            }
        }
    }
        public void AssignPPlayerWeaponAndPassiveItem(List<PlayerInventory.Slot> weapon, List<PlayerInventory.Slot> passiveItems)
    {
        Debug.Log($"Assigning weapons and passives. Weapons: {weapon.Count}, Weapon Icons: {weaponIcon.Count}");
        
        // Assign weapons
        for (int i = 0; i < PlayerweaponIcon.Count; i++)
        {
            if (i < weapon.Count && weapon[i].item != null)
            {
                PlayerweaponIcon[i].sprite = weapon[i].image.sprite;
                PlayerweaponIcon[i].enabled = true;
                Debug.Log($"Assigned weapon {i}: {weapon[i].item.name}");
            }
            else
            {
                PlayerweaponIcon[i].enabled = false;
                Debug.Log($"Disabled weapon slot {i}");
            }
        }

        // Assign passive items
        for (int i = 0; i < PlayerpassiveItemIcon.Count; i++)
        {
            if (i < passiveItems.Count && passiveItems[i].item != null)
            {
                PlayerpassiveItemIcon[i].sprite = passiveItems[i].image.sprite;
                PlayerpassiveItemIcon[i].enabled = true;
                Debug.Log($"Assigned passive {i}: {passiveItems[i].item.name}");
            }   
            else
            {
                PlayerpassiveItemIcon[i].enabled = false;
                Debug.Log($"Disabled passive slot {i}");
            }
        }
    }

    // public void SaveGameData(ref GameData data)
    // {
    //     data.timeSurvival = stopWatchTime;
    // }

    // public void LoadGameData(GameData data)
    // {
    //     stopWatchTime = data.timeSurvival;
    //     DisplayTime();
    // }
    public void UpdateStopWatch()
    {
        stopWatchTime += Time.deltaTime;
        DisplayTime();
        if (stopWatchTime >= TimeLimit)
        {
            foreach (var playerStat in playerStats)
            {
                playerStat.SendMessage("Kill");
            }
            // playerStats.SendMessage("Kill");
        }
    }
    public void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(stopWatchTime / 60);
        float seconds = Mathf.FloorToInt(stopWatchTime % 60);
        stopWacthDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void StartLevelUp()
    {
        Debug.Log("Attempting to start level up...");
        if (playerInventory != null && playerInventory.CheckFullLevelAndSlots())
        {
            Debug.Log("Cannot start level up, inventory is full and all items are at max level.");
            ChangeState(GameState.GamePlay);
            return;
        }

        Debug.Log("Starting level up...");
        ChangeState(GameState.LevelUp);
        // if the level up screen is already active, record that another level up is requested
        if(LevelUpScreen.activeSelf) stackLevelups++;
        else{
            LevelUpScreen.SetActive(true);
            Time.timeScale = 0f;
            stopWacthDisplay.enabled = false;
            // if (playerInventory != null && bOTInventory != null)
            // {
            //     playerInventory.RemoveAndApplyUpgradeOption();
            //     bOTInventory.AutoSelectUpgrade();
            // }
            if(playerInventory != null && bOTInventory == null){
                playerInventory.RemoveAndApplyUpgradeOption();
            }
            else if (playerInventory != null && bOTInventory != null)
            {
                playerInventory.RemoveAndApplyUpgradeOption();
                bOTInventory.AutoSelectUpgrade();
            }
            else
            {
                Debug.LogError("PlayerInventory reference is not set!");
            }
        }
    }
    public void EndLevelUp()
    {
        Time.timeScale = 1f;
        LevelUpScreen.SetActive(false);
        ChangeState(GameState.GamePlay);
        stopWacthDisplay.enabled = true;
        if(stackLevelups > 0) {
            stackLevelups--;
            StartLevelUp();
        };
    }
    public static void GenerateDamageText(string text, Transform target, float duration =1f, float speed = 1f){
        if(!instance.damageTextCanvas) return;
        if(!instance.referenceCamera) instance.referenceCamera = Camera.main;
        instance.StartCoroutine(instance.GenerateDamageTextCoroutine(text, target, duration, speed));
    }
    IEnumerator GenerateDamageTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f) {
        Debug.Log("Coroutine started");

        GameObject textObj = new GameObject("DamageText");
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI textMesh = textObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textMesh.verticalAlignment = VerticalAlignmentOptions.Middle;
        textMesh.fontSize = textFontSize;
        Debug.Log("Text Created: " + text);

        if (textFont) textMesh.font = textFont;
        
        // Ensure the text object is a child of the canvas
        textObj.transform.SetParent(instance.damageTextCanvas.transform, false);

        // Check if target is not destroyed before setting initial position
        if (target != null) {
            rectTransform.position = referenceCamera.WorldToScreenPoint(target.position);
            Debug.Log("referenceCamera Position: " + referenceCamera.transform.position);
            Debug.Log("target Position: " + target.position);
            Debug.Log("Text Position: " + rectTransform.position);
        }

        Destroy(textObj, duration);

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;

        while (t < duration) {
            yield return waitForEndOfFrame;
            t += Time.deltaTime;

            // Check if target still exists before updating position
            if (target == null) {
                // If target is destroyed, exit coroutine
                yield break;
            }

            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1 - t / duration);
            yOffset += speed * Time.deltaTime;

            // Update the position of the damage text relative to the target
            // rectTransform.position = referenceCamera.WorldToScreenPoint(target.position + new Vector3(0, yOffset, 0));
            rectTransform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

        }
    }
    public void AssignBotWeaponAndPassiveItem(List<BOTInventory.Slot> weapon, List<BOTInventory.Slot> passiveItems)
    {
        Debug.Log($"Assigning bot weapons and passives. Weapons: {weapon.Count}, Weapon Icons: {botWeaponIcon.Count}");
        
        // Assign weapons
        for (int i = 0; i < botWeaponIcon.Count; i++)
        {
            if (i < weapon.Count && weapon[i].item != null)
            {
                botWeaponIcon[i].sprite = weapon[i].itemSprite;
                botWeaponIcon[i].enabled = true;
                Debug.Log($"Assigned bot weapon {i}: {weapon[i].item.name}");
            }
            else
            {
                botWeaponIcon[i].enabled = false;
                Debug.Log($"Disabled bot weapon slot {i}");
            }
        }

        // Assign passive items
        for (int i = 0; i < botPassiveItemIcon.Count; i++)
        {
            if (i < passiveItems.Count && passiveItems[i].item != null)
            {
                botPassiveItemIcon[i].sprite = passiveItems[i].itemSprite;
                botPassiveItemIcon[i].enabled = true;
                Debug.Log($"Assigned bot passive {i}: {passiveItems[i].item.name}");
            }
            else
            {
                botPassiveItemIcon[i].enabled = false;
                Debug.Log($"Disabled bot passive slot {i}");
            }
        }
    }
}
