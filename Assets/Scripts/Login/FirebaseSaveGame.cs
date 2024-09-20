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
    public void SaveScoreToFirebase(int score, string characterName, TimeSpan playTime, int level, string playerName)
{
    if (auth != null)
    {
        string userId = auth.CurrentUser.UserId; // Get the current user's ID

        // Lưu điểm số vào node 'scores' của người dùng
        SaveScoreToUserScores(userId, score, characterName, playTime, level, playerName);

        // Lưu điểm số vào node 'globalScores'
        SaveScoreToGlobalScores(score, characterName, playTime, level, playerName );
    }
    else
    {
        Debug.LogError("No user is signed in. Cannot save score data.");
    }
}

// Phương thức lưu điểm số vào 'scores' của người dùng
private void SaveScoreToUserScores(string userId, int score, string characterName, TimeSpan playTime, int level, string playerName)
{
    // Get a reference to the user's scores node
    DatabaseReference userScoresRef = dbReference.Child("users").Child(userId).Child("scores");

    // Push a new score entry with a unique key
    string newScoreKey = userScoresRef.Push().Key;

    // Create the score data including the character name and playtime
    Dictionary<string, object> scoreData = new Dictionary<string, object>
    {
        { "score", score },
        { "characterName", characterName },
        { "playerName", playerName }, // Include the player's name in the score data
        { "playTimeInSeconds", (int)playTime.TotalSeconds }, // Store playtime as total seconds
        { "timestamp", DateTime.UtcNow.ToString("o") }, // Optional: include the time of the score
        { "level", level } // Include the level in the score data
    };

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

// Phương thức lưu điểm số vào 'globalScores'
private void SaveScoreToGlobalScores(int score, string characterName, TimeSpan playTime, int level, string playerName)
{
    // Get a reference to the global scores node
    DatabaseReference globalScoresRef = dbReference.Child("globalScores");

    // Push a new score entry with a unique key
    string newScoreKey = globalScoresRef.Push().Key;

    // Create the score data including the character name and playtime
    Dictionary<string, object> scoreData = new Dictionary<string, object>
    {
        { "score", score },
        { "characterName", characterName },
        { "playerName", playerName }, // Include the player's name in the score data
        { "playTimeInSeconds", (int)playTime.TotalSeconds }, // Store playtime as total seconds
        { "timestamp", DateTime.UtcNow.ToString("o") }, // Optional: include the time of the score
        { "level", level } // Include the level in the score data
    };

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
