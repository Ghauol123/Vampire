using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseSaveGame : MonoBehaviour
{
    private DatabaseReference dbReference; // Database reference
    private FirebaseAuth auth;
    private FirebaseUser user;
    public static FirebaseSaveGame instance; // Singleton pattern

    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // public class PlayerData
    // {
    //     public int Score { get; set; }
    //     public string CharacterName { get; set; }
    //     public string PlayerName { get; set; }
    //     public int Level { get; set; }  // Thêm level vào thông tin người chơi
    // }

    public void SaveScoreToFirebase(PlayerData playerData, PlayerData botData, TimeSpan playTime, string mapName, GameMode gameMode)
    {
        if (auth != null)
        {
            string userId = auth.CurrentUser.UserId;
            SaveScoreToUserScores(userId, playerData, botData, playTime, mapName, gameMode);
            SaveScoreToGlobalScores(playerData, botData, playTime, mapName, gameMode);
        }
        else
        {
            Debug.LogError("No user is signed in. Cannot save score data.");
        }
    }

    private void SaveScoreToUserScores(string userId, PlayerData playerData, PlayerData botData, TimeSpan playTime, string mapName, GameMode gameMode)
    {
        string gameModePath = gameMode.ToString().ToLower();
        DatabaseReference userScoresRef = dbReference.Child("users").Child(userId).Child("scores")
            .Child(gameModePath).Child(mapName);
        
        string newScoreKey = userScoresRef.Push().Key;
        Dictionary<string, object> scoreData = new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow.ToString("o") },
            { "playTimeInSeconds", (int)playTime.TotalSeconds },
            { "gameMode", gameModePath },
            // Thông tin người chơi chính
            { "player", new Dictionary<string, object>
                {
                    { "score", playerData.Score },
                    { "characterName", playerData.CharacterName },
                    { "playerName", playerData.PlayerName },
                    { "level", playerData.Level }
                }
            }
        };

        // Chỉ thêm thông tin bot nếu đang ở chế độ BotMode
        if (gameMode == GameMode.BotMode && botData != null)
        {
            scoreData.Add("bot", new Dictionary<string, object>
            {
                { "score", botData.Score },
                { "characterName", botData.CharacterName },
                { "playerName", botData.PlayerName },
                { "level", botData.Level }
            });
        }

        // Save the new score to Firebase
        userScoresRef.Child(newScoreKey).SetValueAsync(scoreData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Score, character, and playtime saved successfully to user scores.");
            }
            else
            {
                Debug.LogError("Failed to save score data to user scores: " + task.Exception);
            }
        });
    }

    private void SaveScoreToGlobalScores(PlayerData playerData, PlayerData botData, TimeSpan playTime, string mapName, GameMode gameMode)
    {
        string gameModePath = gameMode.ToString().ToLower();
        DatabaseReference globalScoresRef = dbReference.Child("globalScores")
            .Child(gameModePath).Child(mapName);
        
        string newScoreKey = globalScoresRef.Push().Key;
        Dictionary<string, object> scoreData = new Dictionary<string, object>
        {
            { "timestamp", DateTime.UtcNow.ToString("o") },
            { "playTimeInSeconds", (int)playTime.TotalSeconds },
            { "gameMode", gameModePath },
            // Thông tin người chơi chính
            { "player", new Dictionary<string, object>
                {
                    { "score", playerData.Score },
                    { "characterName", playerData.CharacterName },
                    { "playerName", playerData.PlayerName },
                    { "level", playerData.Level }
                }
            }
        };

        // Chỉ thêm thông tin bot nếu đang ở chế độ BotMode
        if (gameMode == GameMode.BotMode && botData != null)
        {
            scoreData.Add("bot", new Dictionary<string, object>
            {
                { "score", botData.Score },
                { "characterName", botData.CharacterName },
                { "playerName", botData.PlayerName },
                { "level", botData.Level }
            });
        }

        // Save the new score to Firebase
        globalScoresRef.Child(newScoreKey).SetValueAsync(scoreData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Score, character, and playtime saved successfully to global scores.");
            }
            else
            {
                Debug.LogError("Failed to save score data to global scores: " + task.Exception);
            }
        });
    }

}
