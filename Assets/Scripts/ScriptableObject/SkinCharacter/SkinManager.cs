
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> skinButtons = new List<GameObject>();

    [SerializeField]
    private List<CharacterData> characterDataList;

    private string userId;
    private DatabaseReference dbReference;

    private void Start()
    {
        // Assume the user is already authenticated and userId is available
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Load purchased skins from Firebase
        LoadPurchasedSkins();
    }

    // private void LoadPurchasedSkins()
    // {
    //     dbReference.Child("users").Child(userId).Child("PurchasedSkins").GetValueAsync().ContinueWithOnMainThread(task =>
    //     {
    //         if (task.IsCompleted)
    //         {
    //             DataSnapshot snapshot = task.Result;

    //             // Loop through all skin buttons
    //             for (int i = skinButtons.Count - 1; i >= 0; i--)
    //             {
    //                 GameObject skinButton = skinButtons[i];
    //                 CostumeData skinCostume = skinButton.GetComponent<CostumeButton>().costumeData;

    //                 // If the skin has already been purchased, remove the button
    //                 if (snapshot.HasChild(skinCostume.name))
    //                 {
    //                     skinButtons.RemoveAt(i);
    //                     Destroy(skinButton);
    //                 }
    //             }
    //         }
    //     });
    // }
private void LoadPurchasedSkins()
{
    dbReference.Child("users").Child(userId).Child("PurchasedSkins").GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            // Loop through all skin buttons
            for (int i = skinButtons.Count - 1; i >= 0; i--)
            {
                GameObject skinButton = skinButtons[i];
                CostumeData skinCostume = skinButton.GetComponent<CostumeButton>().costumeData;
                string characterName = skinCostume.CharacterData.Name;

                // Check if the character and costume exist in the snapshot
                if (snapshot.HasChild(characterName) && snapshot.Child(characterName).HasChild(skinCostume.CostumeName))
                {
                    // Xóa nút trang phục đã mua
                    skinButtons.RemoveAt(i);
                    Destroy(skinButton);
                }
            }
        }
    });
}



    public void PurchaseSkin(GameObject skinButton)
    {
        int index = skinButtons.IndexOf(skinButton);

        if (index != -1)
        {
            CostumeData purchasedCostume = skinButton.GetComponent<CostumeButton>().costumeData;

            foreach (CharacterData character in characterDataList)
            {
                if (character.Name == purchasedCostume.CharacterData.Name)
                {
                    character.costumes.Add(purchasedCostume);
                    break;
                }
            }

            // Save purchased skin to Firebase
            SavePurchasedSkinToFirebase(purchasedCostume);

            // Remove and destroy the skin button
            skinButtons.RemoveAt(index);
            Destroy(skinButton);
        }
    }

    // private void SavePurchasedSkinToFirebase(CostumeData costume)
    // {
    //     dbReference.Child("users").Child(userId).Child("PurchasedSkins").Child(costume.name).SetValueAsync(true);
    // }
private void SavePurchasedSkinToFirebase(CostumeData costume)
{
    // Lấy tên của nhân vật và trang phục
    string characterName = costume.CharacterData.Name; // Tên của nhân vật
    string costumeName = costume.CostumeName;          // Tên của trang phục

    // Thay vì lưu true, lưu một đối tượng JSON đơn giản chứa các thuộc tính cần thiết
    var costumeData = new Dictionary<string, object>
    {
        { "costumeName", costumeName }
        // Nếu cần, bạn có thể thêm các thuộc tính khác của costume vào đây
    };

    // Lưu vào Firebase theo cấu trúc /users/{userId}/PurchasedSkins/{CharacterName}/{CostumeName}
    dbReference.Child("users").Child(userId).Child("PurchasedSkins").Child(characterName).Child(costumeName).SetValueAsync(costumeData);
}


}

