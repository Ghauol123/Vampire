using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks; // Add this for async/await

public class CharacterButtonHandler : CharacterLoadLevel
{
    public GameObject characterButtonPrefab; // Assign this in the Unity Editor
    public Transform buttonParent; // Assign the parent transform in the Unity Editor
    private FirebaseController firebaseController;
    private Dictionary<int, TextMeshProUGUI> levelTexts = new Dictionary<int, TextMeshProUGUI>();
    [SerializeField]
    private TextMeshProUGUI goldText; // UI element to display current gold
    private int currentGold;

    private async void Start()
    {
        firebaseController = FirebaseController.instance;
        currentGold = await FirebaseLoadCoin.instance.GetCurrentCoinFromFirebase(); // Get gold from Firebase
        for (int i = 0; i < characters.Count; i++)
        {
            int index = i; // Capture the current index
            Debug.Log(i);
            // Instantiate a new button from the prefab
            GameObject buttonObject = Instantiate(characterButtonPrefab, buttonParent);
            Button characterButton = buttonObject.GetComponent<Button>();
            
            // Get the CostumeButton component
            CostumeButton costumeButton = buttonObject.GetComponent<CostumeButton>();
            if (costumeButton != null)
            {
                // Assign costumeData to the component
                costumeButton.costumeData = characters[index].costumes[0];
                costumeButton.costumeImage.sprite = characters[index].costumes[0].CostumeSprite;
                costumeButton.SetupSpriteAnimation();
            }

            // Get the TextMeshPro component and set the character level
            TextMeshProUGUI levelText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
            if (levelText != null)
            {
                levelText.text = $"Level: {characters[index].Level}";
                levelTexts[index] = levelText; // Store the reference
            }

            // Set up the button listener
            characterButton.onClick.AddListener(() => OnCharacterButtonClicked(index));
        }
    }


    private async void OnCharacterButtonClicked(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characters.Count)
        {
            Debug.LogError("Invalid character index.");
            return;
        }

        CharacterData character = characters[characterIndex];
        int upgradeCost = 1 * character.Level;

        if (character.Level >= 7)
        {
            Debug.Log($"Character {character.Name} has reached the maximum level.");
            return;
        }

        if (currentGold >= upgradeCost)
        {
            currentGold -= upgradeCost;
            await FirebaseLoadCoin.instance.UpdateCoinInFirebase(currentGold); // Update gold in Firebase
            character.UpgradeLevel();
            character.AdjustStatsBasedOnLevel(); // Adjust stats after upgrading the level
            Debug.Log($"Character {character.Name} upgraded to level {character.Level}");

            // Update the level text
            if (levelTexts.TryGetValue(characterIndex, out TextMeshProUGUI levelText))
            {
                levelText.text = $"Level: {character.Level}";
            }

            // Save the new level to Firebase
            firebaseController.SaveCharacterLevel(character.Name, character.Level);

            // Update gold display
            goldText.text = $"Gold: {currentGold}";
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade {character.Name}. Required: {upgradeCost}, Available: {currentGold}");
        }
    }
}
