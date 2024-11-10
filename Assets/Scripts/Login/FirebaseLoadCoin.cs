using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class FirebaseLoadCoin : MonoBehaviour
{
    public static FirebaseLoadCoin instance;
    private DatabaseReference dbReference;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async Task<int> GetCurrentCoinFromFirebase()
    {
        string userId = FirebaseController.instance.userId;
        
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("GetCurrentCoinFromFirebase: No user logged in");
            return 0;
        }

        try
        {
            DatabaseReference coinRef = dbReference.Child("users").Child(userId).Child("coin");
            var snapshot = await coinRef.GetValueAsync();

            if (snapshot.Exists)
            {
                int coin = int.Parse(snapshot.Value.ToString());
                Debug.Log($"Current coins: {coin}");
                return coin;
            }
            
            Debug.Log("No coin data found. Returning default value: 0");
            return 0;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting coin data: {e.Message}");
            return 0;
        }
    }

    public async Task UpdateCoinInFirebase(int newCoinValue)
    {
        string userId = FirebaseController.instance.userId;

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("UpdateCoinInFirebase: No user logged in");
            return;
        }

        try
        {
            DatabaseReference coinRef = dbReference.Child("users").Child(userId).Child("coin");
            await coinRef.SetValueAsync(newCoinValue);
            Debug.Log($"Coin updated successfully: {newCoinValue}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating coin: {e.Message}");
        }
    }
}
