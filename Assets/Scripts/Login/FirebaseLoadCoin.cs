using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class FirebaseLoadCoin : MonoBehaviour
{
    public static FirebaseLoadCoin instance;
    private DatabaseReference dbReference; // Database reference
    private FirebaseAuth auth; // Firebase authentication reference
    private string userId; // User ID

    // Start is called before the first frame update
    async void Start() // Đánh dấu phương thức là async để có thể gọi các hàm bất đồng bộ
    {        
                if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed on scene load
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        // Thiết lập Firebase Auth và lấy UserId của người dùng hiện tại
        auth = FirebaseAuth.DefaultInstance;
        
        // Kiểm tra nếu có người dùng đăng nhập
        if (auth.CurrentUser != null)
        {
            userId = auth.CurrentUser.UserId;
            Debug.Log("User ID: " + userId);
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Gọi hàm GetCurrentCoinFromFirebase khi game bắt đầu
            int currentCoin = await GetCurrentCoinFromFirebase();
            Debug.Log("Số coin nhận được khi bắt đầu game: " + currentCoin);
        }
        else
        {
            Debug.LogError("Không có người dùng đăng nhập.");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public async Task<int> GetCurrentCoinFromFirebase()
    {
        if (auth != null && !string.IsNullOrEmpty(userId))
        {
            DatabaseReference coinRef = dbReference.Child("users").Child(userId).Child("coin");

            var task = coinRef.GetValueAsync();
            await task;

            if (task.IsCompleted && task.Result.Exists)
            {
                // Lấy giá trị coin từ Firebase
                int coin = int.Parse(task.Result.Value.ToString());

                // Log giá trị coin ra console
                Debug.Log($"Số coin hiện tại trong Firebase: {coin}");

                return coin;
            }
            else
            {
                // Nếu không có giá trị coin, trả về 0 và log ra console
                Debug.Log("Không tìm thấy dữ liệu coin trong Firebase. Giá trị mặc định: 0");

                return 0;
            }
        }
        else
        {
            Debug.LogError("Không có người dùng đăng nhập hoặc userId không hợp lệ.");
            return 0;
        }
    }
        public async Task UpdateCoinInFirebase(int newCoinValue)
{
    if (auth != null)
    {
        DatabaseReference coinRef = dbReference.Child("users").Child(userId).Child("coin");

        // Cập nhật giá trị coin mới lên Firebase
        await coinRef.SetValueAsync(newCoinValue);
        Debug.Log("Cập nhật số coin thành công: " + newCoinValue);
    }
    else
    {
        Debug.LogError("No user signed in. Không thể cập nhật coin.");
    }
}
}
