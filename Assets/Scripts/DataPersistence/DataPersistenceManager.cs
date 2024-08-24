using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    public static DataPersistenceManager instance;
    public GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandle fileDataHandle;
    PlayerStats playerStats;
    public bool isNewgame = false;
    private DatabaseReference databaseReference;


    private void Awake()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        if(instance != null)
        {
            Debug.LogWarning("There is more than one instance of DataPersistenceManager");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        fileDataHandle = new FileDataHandle(Application.persistentDataPath, fileName);
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            string uid = auth.CurrentUser.UserId;
            databaseReference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(uid);
        }

    }
    public void NewGameData()
    {
        gameData = new GameData();
    }
    // public void SaveGameData()
    // {
    //     if(gameData == null){
    //         Debug.LogWarning("gameData is null");
    //         return;
    //     }
    //       // Find the PlayerStats instance
    //     playerStats = FindObjectOfType<PlayerStats>();

    //     // Check if PlayerStats is active and not null
    //     if (playerStats == null || !playerStats.gameObject.activeInHierarchy)
    //     {
    //         Debug.LogWarning("PlayerStats is not active, skipping SaveGameData");
    //         return;
    //     }
    //     this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    //     foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
    //     {
    //         dataPersistenceObject.SaveGameData(ref gameData);
    //     }
    //     Debug.Log("Saving game data");
    //     Debug.Log("Player position: " + gameData.playerPosition);

    //     fileDataHandle.Save(gameData);
    // }
    // public void LoadGameData()
    // {
    //     gameData = fileDataHandle.Load(gameData);
    //     if(gameData == null){
    //         Debug.LogWarning("gameData is null");
    //         return;
    //     }
    //     this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    //     foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
    //     {
    //         dataPersistenceObject.LoadGameData(gameData);
    //     }
    //     Debug.Log("Loading game data");
    // }
    public void SaveGameData()
    {
        if (gameData == null)
        {
            Debug.LogWarning("gameData là null");
            return;
        }

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveGameData(ref gameData);
        }

        string jsonData = JsonUtility.ToJson(gameData);
        databaseReference.Child("gameSave").SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Dữ liệu game đã được lưu thành công vào Firebase.");
            }
            else
            {
                Debug.LogWarning("Không thể lưu dữ liệu game: " + task.Exception);
            }
        });
    }
    public void LoadGameData()
{
    if (databaseReference == null)
    {
        Debug.LogWarning("Người dùng chưa được xác thực.");
        return;
    }

    databaseReference.Child("gameSave").GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                string jsonData = snapshot.GetRawJsonValue();
                gameData = JsonUtility.FromJson<GameData>(jsonData);

                this.dataPersistenceObjects = FindAllDataPersistenceObjects();
                foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
                {
                    dataPersistenceObject.LoadGameData(gameData);
                }
                Debug.Log("Dữ liệu game đã được tải thành công từ Firebase.");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy dữ liệu lưu cho người dùng này.");
            }
        }
        else
        {
            Debug.LogWarning("Không thể tải dữ liệu game: " + task.Exception);
        }
    });
}
    public void OnUserChanged()
{
    FirebaseAuth auth = FirebaseAuth.DefaultInstance;
    if (auth.CurrentUser != null)
    {
        string uid = auth.CurrentUser.UserId;
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(uid);
        LoadGameData();  // Tải dữ liệu game của người dùng mới
    }
    else
    {
        Debug.LogWarning("Không có người dùng nào đăng nhập.");
        gameData = null;
    }
}

    private void OnApplicationQuit() {
        SaveGameData();
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}