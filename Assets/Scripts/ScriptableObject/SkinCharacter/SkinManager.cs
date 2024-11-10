using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // Make sure to include this for UI elements

public class SkinManager : MonoBehaviour
{
    [SerializeField]
    private GameObject skinButtonPrefab;  // Prefab for the skin button

    [SerializeField]
    private Transform skinButtonContainer;  // Parent transform to hold the instantiated buttons

    [SerializeField]
    private List<CostumeData> allCostumes = new List<CostumeData>();  // List of all available costumes

    private List<GameObject> skinButtons = new List<GameObject>();

    [SerializeField]
    private List<CharacterData> characterDataList;

    [SerializeField]
    private GameObject confirmationPanel; // Reference to the confirmation panel
    [SerializeField]
    private Button yesButton;             // Yes button on the panel
    [SerializeField]
    private Button noButton;              // No button on the panel

    private GameObject selectedSkinButton; // The skin button currently being selected
    private string userId;
    private DatabaseReference dbReference;
    [SerializeField]
    private TextMeshProUGUI coin;
    [SerializeField]
    private GameObject successPanel;  // Panel to show success message
    [SerializeField]
    private GameObject errorPanel;    // Panel to show error message

    private int currentCoin;

    private void Start()
    {
        // Assume the user is already authenticated and userId is available
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Load purchased skins and create buttons
        LoadPurchasedSkinsAndCreateButtons();

        // Hide confirmation panel by default
        confirmationPanel.SetActive(false);

        // Add listeners to the Yes and No buttons
        yesButton.onClick.AddListener(ConfirmPurchase);
        noButton.onClick.AddListener(CancelPurchase);

        // Hide success and error panels by default
        successPanel.SetActive(false);
        errorPanel.SetActive(false);

        UpdateCoinDisplay();
    }

    private void LoadPurchasedSkinsAndCreateButtons()
    {
        dbReference.Child("users").Child(userId).Child("PurchasedSkins").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                foreach (CostumeData costume in allCostumes)
                {
                    string characterName = costume.CharacterData.Name;
                    string costumeName = costume.CostumeName;

                    // Check if the costume is not already purchased
                    if (!snapshot.HasChild(characterName) || !snapshot.Child(characterName).HasChild(costumeName))
                    {
                        // Create a new button for this costume
                        GameObject newButton = Instantiate(skinButtonPrefab, skinButtonContainer);
                        CostumeButton costumeButton = newButton.GetComponent<CostumeButton>();

                        if (costumeButton != null)
                        {
                            costumeButton.costumeData = costume;
                            costumeButton.SetupButton();  // You'll need to implement this method in CostumeButton
                        }

                        // Add a click listener to the button
                        Button buttonComponent = newButton.GetComponent<Button>();
                        if (buttonComponent != null)
                        {
                            buttonComponent.onClick.AddListener(() => PurchaseSkin(newButton));
                        }

                        skinButtons.Add(newButton);
                    }
                }
            }
        });
    }

    private async void UpdateCoinDisplay()
    {
        // Check if FirebaseLoadCoin instance exists and user is signed in
        if (FirebaseLoadCoin.instance != null && FirebaseController.instance.userId != null)
        {
            try
            {
                // Get the current coin value from FirebaseLoadCoin
                currentCoin = await FirebaseLoadCoin.instance.GetCurrentCoinFromFirebase();

                // Update the coinText UI element with the current coin value
                coin.text = $"Coins: {currentCoin}";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating coin display: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("FirebaseLoadCoin instance or userId is null.");
        }
    }

    public void PurchaseSkin(GameObject skinButton)
    {
        selectedSkinButton = skinButton;

        // Show the confirmation panel
        confirmationPanel.SetActive(true);
    }

    private async void ConfirmPurchase()
    {
        int index = skinButtons.IndexOf(selectedSkinButton);

        if (index != -1)
        {
            CostumeData purchasedCostume = selectedSkinButton.GetComponent<CostumeButton>().costumeData;

            // Check if the player has enough coins
            if (currentCoin >= purchasedCostume.Price)
            {
                // Deduct the price of the skin from the player's coins
                currentCoin -= purchasedCostume.Price;

                try
                {
                    // Update the Firebase database with the new coin value
                    await FirebaseLoadCoin.instance.UpdateCoinInFirebase(currentCoin);

                    // Add the purchased costume to the character's costume list
                    foreach (CharacterData character in characterDataList)
                    {
                        if (character.Name == purchasedCostume.CharacterData.Name)
                        {
                            character.costumes.Add(purchasedCostume);
                            break;
                        }
                    }

                    // Save purchased skin to Firebase
                    await SavePurchasedSkinToFirebase(purchasedCostume);

                    // Remove and destroy the skin button
                    skinButtons.RemoveAt(index);
                    Destroy(selectedSkinButton);

                    // Hide the confirmation panel
                    confirmationPanel.SetActive(false);

                    // Show success panel and hide it after 2 seconds
                    successPanel.SetActive(true);
                    Invoke(nameof(HideSuccessPanel), 2f);

                    // Update coin display
                    UpdateCoinDisplay();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error confirming purchase: {e.Message}");
                    // Show error panel
                    errorPanel.SetActive(true);
                    Invoke(nameof(HideErrorPanel), 2f);
                }
            }
            else
            {
                // Not enough coins, show error panel and hide it after 2 seconds
                errorPanel.SetActive(true);
                confirmationPanel.SetActive(false);
                Invoke(nameof(HideErrorPanel), 2f);
            }
        }
    }

    private void CancelPurchase()
    {
        // Hide the confirmation panel without purchasing the skin
        confirmationPanel.SetActive(false);
    }

    private async Task SavePurchasedSkinToFirebase(CostumeData costume)
    {
        // Get character name and costume name
        string characterName = costume.CharacterData.Name;
        string costumeName = costume.CostumeName;

        // Save the costume data to Firebase
        var costumeData = new Dictionary<string, object>
        {
            { "costumeName", costumeName }
        };

        await dbReference.Child("users").Child(userId).Child("PurchasedSkins").Child(characterName).Child(costumeName).SetValueAsync(costumeData);
    }

    private void HideSuccessPanel()
    {
        successPanel.SetActive(false);
    }

    private void HideErrorPanel()
    {
        errorPanel.SetActive(false);
    }
}
