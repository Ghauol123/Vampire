using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseLoadSkin : MonoBehaviour
{
    private DatabaseReference dbReference;
    private string userId;
    private CharacterManager characterManager;

    void Start()
    {
        userId = FirebaseController.instance.userId;
        Debug.Log("User ID: " + userId);
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Find CharacterManager in the scene
        characterManager = FindObjectOfType<CharacterManager>();

        // Load purchased skins from Firebase
        LoadPurchasedSkins();
    }

    private void LoadPurchasedSkins()
    {
        ClearPreviousAccountCostumes();
        dbReference.Child("users").Child(userId).Child("PurchasedSkins").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                
                Dictionary<string, List<string>> purchasedSkins = new Dictionary<string, List<string>>();

                foreach (DataSnapshot characterSnapshot in snapshot.Children)
                {
                    string characterName = characterSnapshot.Key;
                    List<string> costumes = new List<string>();

                    foreach (DataSnapshot costumeSnapshot in characterSnapshot.Children)
                    {
                        string costumeName = costumeSnapshot.Key;
                        costumes.Add(costumeName);
                    }
                    foreach (var costume in costumes)
                    {
                        Debug.Log("Purchased skin: " + characterName + " - " + costume);
                    }
                    purchasedSkins[characterName] = costumes;
                }

                UpdateCharacterManager(purchasedSkins);
            }
            else
            {
                Debug.LogError("Failed to load purchased skins: " + task.Exception);
            }
        });
    }
    private void UpdateCharacterManager(Dictionary<string, List<string>> purchasedSkins)
{
    if (characterManager != null)
    {
        foreach (var characterEntry in purchasedSkins)
        {
            string characterName = characterEntry.Key;
            List<string> costumeNames = characterEntry.Value;

            CharacterData characterData = characterManager.GetCharacterData(characterName);

            if (characterData != null)
            {
                // Clear existing costumes except the default costume at index 0
                var defaultCostume = characterData.costumes[0];
                characterData.costumes.Clear();
                characterData.costumes.Add(defaultCostume);

                // Add new costumes starting from index 1
                foreach (string costumeName in costumeNames)
                {
                    CostumeData costumeData = characterManager.GetCostumeData(costumeName);
                    if (costumeData != null)
                    {
                        characterData.costumes.Add(costumeData);
                    }
                    else
                    {
                        Debug.LogWarning("Costume not found: " + costumeName);
                    }
                }

                // Update the character's costumes
                characterManager.UpdateCharacterCostumes(characterData);
            }
            else
            {
                Debug.LogWarning("CharacterData not found for character: " + characterName);
            }
        }
    }
    else
    {
        Debug.LogError("CharacterManager not found.");
    }
}
private void ClearPreviousAccountCostumes()
{
    if (characterManager != null)
    {
        foreach (var characterData in characterManager.GetAllCharacters())
        {
            // Keep the default costume at index 0, remove others
            if (characterData.costumes.Count > 1)
            {
                var defaultCostume = characterData.costumes[0];
                characterData.costumes.Clear();
                characterData.costumes.Add(defaultCostume);
            }
        }
    }
    else
    {
        Debug.LogError("CharacterManager not found.");
    }
}
}