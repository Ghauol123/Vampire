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

public class GameManager : MonoBehaviour
{
public static GameManager instance;
public bool isGameLoaded;
    public GameState currentState;
    public GameState previousState;
    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject ResultScreen;
    public GameObject gameOverScreen;
    public GameObject LevelUpScreen;
    public int stackLevelups = 0;
    [Header("Current Stas Displays")]
    // public TMP_Text CurrentSpeedDisplay;
    // public TMP_Text CurrentHealDisplay;
    // public TMP_Text CurrentMightDisplay;
    // public TMP_Text CurrentProjectTileSpeedDisplay;
    // public TMP_Text CurrentrecoveryDisplay;
    // public TMP_Text CurrentMagnetDisplay;
    public TMP_Text levelReach;
    public TMP_Text ScoreEndGame;
    public TMP_Text timeSurvival;
    [Header("Result Screen Displays")]
    public Image characterImage;
    public TMP_Text characterName;
    public Image IconCharacter;
    public List<Image> weaponIcon;
    public List<Image> passiveItemIcon;
    [Header("Time")]
    public float TimeLimit;
    public float stopWatchTime;
    public TMP_Text stopWacthDisplay;

    public bool IsGamePause = false;
    public bool IsLevelUp = false;

    public bool IsGameOver {get{return currentState == GameState.GameOver;}}
    public bool chosingUpgrade {get{return currentState == GameState.LevelUp;}}

    [Header("Damage Text Setting")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20f;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;
    // public GameObject player;
    PlayerStats[] playerStats;
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

    // PlayerStats[] playerStats;
    FirebaseSaveGame firebaseSaveGame;
    private PlayerInventory playerInventory;

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
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory not found!");
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
    }
public async void GameOver()
{
    // Display the game over screen
    stopWacthDisplay.enabled = false;
    timeSurvival.text = stopWacthDisplay.text;
    ChangeState(GameState.GameOver);
    Time.timeScale = 0f;
    gameOverScreen.SetActive(true);
    DisplayResultScreen();
    
    int finalScore = CalculateFinalScore(); // Replace with your actual score calculation logic

    // Save score to Firebase
    TimeSpan timeSpan = TimeSpan.FromSeconds(stopWatchTime);
    
    // Assuming you have a way to get the player's name, for example from Firebase auth or player data
    string playerName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName; // If using Firebase Authentication

    int firebaseCoin = await FirebaseLoadCoin.instance.GetCurrentCoinFromFirebase();

    // Lấy số coin từ game (giả sử bạn có biến chứa số coin từ game)
    int gameCoin = 0;
    foreach (var playerStat in playerStats)
    {
        gameCoin += playerStat.coin; // playerStats.coin là số coin người chơi kiếm được trong game
    }

    // Tổng số coin
    int totalCoin = firebaseCoin + gameCoin;

    // Cập nhật số coin mới lên Firebase
    await FirebaseLoadCoin.instance.UpdateCoinInFirebase(totalCoin);


    // If you're storing the player's name elsewhere, retrieve it accordingly
    int totalScore = 0;
    string characterName = "";
    int totalLevel = 0;
    
    foreach (var playerStat in playerStats)
    {
        totalScore += playerStat.score;
        characterName = playerStat.cst.Name; // Assuming all playerStats have the same character name
        totalLevel += playerStat.level;
    }
    
    firebaseSaveGame.SaveScoreToFirebase(totalScore, characterName, timeSpan, totalLevel, playerName);
}

    private int CalculateFinalScore()
    {
        if (PlayerStats.instance == null)
        {
            Debug.LogError("PlayerStats instance is null!");
            return 0; // Trả về giá trị mặc định nếu instance bị null
        }

        // Lấy điểm từ PlayerStats
        int score = PlayerStats.instance.score;

        // Kiểm tra điểm
        Debug.Log("Final Score from PlayerStats: " + score);

        return score;
    }
    public void DisplayResultScreen()
    // Display the result screen
    {
        ResultScreen.SetActive(true);
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
    // Assign the weapon and passive item to the result screen
    {
        if (weapon.Count != weaponIcon.Count || passiveItems.Count != passiveItemIcon.Count)
        {
            Debug.Log("Chosen weapon and passiveItem data list have different lenghts");
            return;
        }
        for (int i = 0; i < weaponIcon.Count; i++)
        {
            if (weapon[i].image.sprite)
            {
                weaponIcon[i].sprite = weapon[i].image.sprite;
                weaponIcon[i].enabled = true;
            }
            else
            {
                weaponIcon[i].enabled = false;
            }
        }
        for (int i = 0; i < passiveItemIcon.Count; i++)
        {
            if (passiveItems[i].image.sprite)
            {
                passiveItemIcon[i].sprite = passiveItems[i].image.sprite;
                passiveItemIcon[i].enabled = true;
            }
            else
            {
                passiveItemIcon[i].enabled = false;
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
            foreach (var playerStat in playerStats)
            {
                playerStat.SendMessage("RemoveAndApplyUpgradeOption");
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
}
