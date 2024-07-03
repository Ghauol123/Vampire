using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState currentState;
    public GameState previousState;
    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject ResultScreen;
    public GameObject gameOverScreen;
    public GameObject LevelUpScreen;
    public int stackLevelups = 0;
    [Header("Current Stas Displays")]
    public TMP_Text CurrentSpeedDisplay;
    public TMP_Text CurrentHealDisplay;
    public TMP_Text CurrentMightDisplay;
    public TMP_Text CurrentProjectTileSpeedDisplay;
    public TMP_Text CurrentrecoveryDisplay;
    public TMP_Text CurrentMagnetDisplay;
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


    // public GameObject player;
    PlayerStats playerStats;
    // Start is called before the first frame update
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
        playerStats = FindObjectOfType<PlayerStats>();
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
    // Change the current state of the game
    {
        previousState = currentState;
        currentState = newState;
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
    public void GameOver()
    // Display the game over screen
    {
        stopWacthDisplay.enabled = false;
        timeSurvival.text = stopWacthDisplay.text;
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        DisplayResultScreen();
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
    public void UpdateStopWatch()
    {
        stopWatchTime += Time.deltaTime;
        DisplayTime();
        if (stopWatchTime >= TimeLimit)
        {
            playerStats.SendMessage("Kill");
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
        ChangeState(GameState.LevelUp);
        // if the level up screen is already active, record that another level up is requested
        if(LevelUpScreen.activeSelf) stackLevelups++;
        else{
            LevelUpScreen.SetActive(true);
            Time.timeScale = 0f;
            stopWacthDisplay.enabled = false;
            playerStats.SendMessage("RemoveAndApplyUpgradeOption");
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
}
