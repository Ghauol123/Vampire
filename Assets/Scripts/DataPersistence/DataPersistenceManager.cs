using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }
    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    //     SceneManager.sceneUnloaded += OnSceneUnloaded;
    // }
    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    //     SceneManager.sceneUnloaded -= OnSceneUnloaded;
    // }

    // public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    // {
    //     Debug.Log("Scene loaded");
    //     this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    //     // LoadGameData();
    // }
    // public void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
    // {
    //     Debug.Log("Scene unloaded");
    //     SaveGameData();
    // }
    public void NewGameData()
    {
        gameData = new GameData();
    }
    public void SaveGameData()
    {
        if(gameData == null){
            Debug.LogWarning("gameData is null");
            return;
        }
          // Find the PlayerStats instance
        playerStats = FindObjectOfType<PlayerStats>();

        // Check if PlayerStats is active and not null
        if (playerStats == null || !playerStats.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("PlayerStats is not active, skipping SaveGameData");
            return;
        }
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.SaveGameData(ref gameData);
        }
        Debug.Log("Saving game data");
        Debug.Log("Player position: " + gameData.playerPosition);
        fileDataHandle.Save(gameData);
    }
    public void LoadGameData()
    {
        gameData = fileDataHandle.Load();
        if(gameData == null){
            Debug.LogWarning("gameData is null");
            return;
        }
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            dataPersistenceObject.LoadGameData(gameData);
        }
        Debug.Log("Loading game data");
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
