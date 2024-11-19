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
    private Dictionary<int, TextMeshProUGUI> upgradeCostTexts = new Dictionary<int, TextMeshProUGUI>();
    [SerializeField]
    private TextMeshProUGUI goldText; // UI element to display current gold
    private int currentGold;
    public int upgradeCost;

    private async void Start()
    {
        firebaseController = FirebaseController.instance;
        characters = firebaseController.characterDataList;
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

            // Get the TextMeshPro components for level and upgrade cost
            TextMeshProUGUI[] texts = buttonObject.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = $"Level: {characters[index].Level}";
                texts[1].text = $"Upgrade: {characters[index].UpgradeMoney}";
                levelTexts[index] = texts[0];
                upgradeCostTexts[index] = texts[1];
            }

            // Set up the button listener
            characterButton.onClick.AddListener(() => OnCharacterButtonClicked(index));
        }
        
        UpdateGoldDisplay();
    }


    private async void OnCharacterButtonClicked(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characters.Count)
        {
            Debug.LogError("Invalid character index.");
            return;
        }

        CharacterData character = characters[characterIndex];
        int upgradeCost = character.UpgradeMoney;

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

            UpdateCharacterDisplay(characterIndex);
            firebaseController.SaveCharacterLevel(character.Name, character.Level);
            UpdateGoldDisplay();
        }
        else
        {
            Debug.Log($"Not enough gold to upgrade {character.Name}. Required: {upgradeCost}, Available: {currentGold}");
        }
    }

    private void UpdateCharacterDisplay(int characterIndex)
    {
        CharacterData character = characters[characterIndex];
        if (levelTexts.TryGetValue(characterIndex, out TextMeshProUGUI levelText))
        {
            levelText.text = $"Level: {character.Level}";
        }
        if (upgradeCostTexts.TryGetValue(characterIndex, out TextMeshProUGUI upgradeCostText))
        {
            upgradeCostText.text = $"Upgrade: {character.UpgradeMoney}";
        }
    }

    private void UpdateGoldDisplay()
    {
        goldText.text = $"Gold: {currentGold}";
    }
}
