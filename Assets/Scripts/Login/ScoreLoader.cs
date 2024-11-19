using Firebase.Database;
using Firebase.Auth;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Extensions;
using System;
using UnityEngine.UI;

public class ScoreLoader : MonoBehaviour
{
    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    private FirebaseUser user;

    public GameObject scoreScrollViewContent;  // Content của ScrollView
    public GameObject scoreEntryPrefab;       // Prefab cho mỗi mục điểm số
    public Button toggleButton;               // Nút chuyển đổi giữa điểm cá nhân và toàn cầu
    // public TMP_InputField searchInputField;   // Thanh tìm kiếm theo tên người chơi
    private bool isPersonalScores = true;     // Biến để theo dõi trạng thái hiện tại
    private List<Dictionary<string, object>> allScores = new List<Dictionary<string, object>>();  // Lưu tất cả điểm
    public Button btnpanelChar;                  // Panel chứa các nút chọn nhân vật
    public GameObject panelChar;
    private TextMeshProUGUI btnpanelCharText;
    public Image characterImage; // Thêm trường này để tham chiếu đến Image component
    
    // Dictionary để lưu trữ các Sprite của nhân vật
    private Dictionary<string, Sprite> characterSprites = new Dictionary<string, Sprite>();

    private const string DEFAULT_CHARACTER = "Amelia Watson";

    // Add new UI elements
    public Button gameModeToggleButton;
    public Button mapToggleButton;
    public TextMeshProUGUI gameModeText;
    public TextMeshProUGUI mapText;

    // Add new state tracking variables
    private GameMode currentGameMode = GameMode.SinglePlayer;
    private string currentMap = "Easy"; // Default map
    
    // Define available maps
    private readonly string[] availableMaps = { "Easy", "Hard" };

    // Thêm biến mới
    private TextMeshProUGUI toggleButtonText;

    // Thêm biến để lưu tên nhân vật hiện tại
    private string currentCharacterName = DEFAULT_CHARACTER;

    void Start()
    {
        // Khởi tạo Firebase
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        // // Gán sự kiện OnValueChanged cho thanh tìm kiếm
        // searchInputField.onValueChanged.AddListener(OnSearchValueChanged);

        // // Ẩn thanh tìm kiếm ban đầu
        // searchInputField.gameObject.SetActive(false);

        // Tải điểm cá nhân ban đầu với nhân vật mặc định
        LoadScoresByCharacterName(DEFAULT_CHARACTER);
        
        panelChar.SetActive(false);
        LoadPersonalScoresForMode();
        // Initialize the button text component
        btnpanelCharText = btnpanelChar.GetComponentInChildren<TextMeshProUGUI>();
        if (btnpanelCharText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on btnpanelChar. Please check the button setup.");
        }
        
        // Set default text
        if (btnpanelCharText != null)
        {
            btnpanelCharText.text = DEFAULT_CHARACTER;
        }

        // Add click listener to the character panel button
        btnpanelChar.onClick.AddListener(ToggleCharacterPanel);    
                if (gameModeToggleButton != null)
            gameModeToggleButton.onClick.AddListener(ToggleGameMode);
        if (mapToggleButton != null)
            mapToggleButton.onClick.AddListener(ToggleMap);

        // Initialize text displays
        UpdateGameModeText();
        UpdateMapText();

        // Thêm listener cho toggle button
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleScoreView);
        }

        LoadScoresForCurrentSelection();

        // Lấy component TextMeshProUGUI từ toggle button
        toggleButtonText = toggleButton.GetComponentInChildren<TextMeshProUGUI>();
        if (toggleButtonText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on toggle button");
        }

        // Cập nhật text ban đầu
        UpdateToggleButtonText();
    }

    // New method to toggle the character panel
    private void ToggleCharacterPanel()
    {
        if (panelChar.activeSelf)
        {
            closePanelChar();
        }
        else
        {
            openPanelChar();
        }
    }

    public void openPanelChar()
    {
        panelChar.SetActive(true);
    }
    public void closePanelChar()
    {
        panelChar.SetActive(false);
    }

    // Phương thức lưu tất cả điểm vào danh sách và hiển thị
    private void PopulateScoreList(DataSnapshot snapshot, bool isPersonalScores)
    {
        allScores.Clear();

        // Kiểm tra nếu snapshot không có dữ liệu
        if (!snapshot.Exists)
        {
            Debug.Log("No scores found for current selection");
            DisplayScores(allScores); // Hiển thị danh sách rỗng
            return;
        }

        foreach (DataSnapshot scoreSnapshot in snapshot.Children)
        {
            Dictionary<string, object> scoreData = new Dictionary<string, object>();
            
            // Lấy dữ liệu người chơi
            var playerData = scoreSnapshot.Child("player").Value as Dictionary<string, object>;
            if (playerData != null && 
                playerData["characterName"].ToString() == currentCharacterName) // Sử dụng biến currentCharacterName
            {
                scoreData["score"] = playerData["score"];
                scoreData["characterName"] = playerData["characterName"];
                scoreData["playerName"] = playerData["playerName"];
                scoreData["level"] = playerData["level"];
                scoreData["playTimeInSeconds"] = scoreSnapshot.Child("playTimeInSeconds").Value;
                allScores.Add(scoreData);
            }
        }

        // Thêm debug logs
        Debug.Log($"Found {allScores.Count} scores for {currentGameMode} mode in {currentMap} map");
        foreach (var score in allScores)
        {
            Debug.Log($"Score: {score["score"]}, Player: {score["playerName"]}, Character: {score["characterName"]}");
        }

        DisplayScores(allScores);
    }

    // Phương thức hiển thị điểm từ danh sách đã lọc
    private void DisplayScores(List<Dictionary<string, object>> scoreList)
    {
        // Xóa nội dung cũ
        foreach (Transform child in scoreScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Sắp xếp danh sách điểm số theo thứ tự từ cao xuống thấp
        scoreList.Sort((x, y) => Convert.ToInt32(y["score"]).CompareTo(Convert.ToInt32(x["score"])));

        // Tạo các mục điểm số mới theo thứ tự đã sắp xếp và hiển thị rank
        int rank = 1;
        foreach (Dictionary<string, object> scoreData in scoreList)
        {
            GameObject scoreEntry = Instantiate(scoreEntryPrefab, scoreScrollViewContent.transform);

            TextMeshProUGUI rankText = scoreEntry.transform.Find("Rank Level Text").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = scoreEntry.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = scoreEntry.transform.Find("CharacterText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playTimeText = scoreEntry.transform.Find("PlayTimeText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI leveltext = scoreEntry.transform.Find("Level Text").GetComponent<TextMeshProUGUI>();

            // Update rank, score, name, and playtime text
            rankText.text = "" + rank;
            scoreText.text = "Score: " + scoreData["score"].ToString();
            playTimeText.text = "Time: " + scoreData["playTimeInSeconds"].ToString() + "s";
            leveltext.text = "LV:" + scoreData["level"].ToString();

            // Hiển thị thông tin khác nhau dựa trên chế độ
            if (isPersonalScores)
            {
                nameText.text = scoreData["characterName"].ToString();
            }
            else
            {
                // Hiển thị cả tên người chơi và nhân vật trong chế độ toàn cầu
                string playerName = scoreData["playerName"]?.ToString() ?? "Unknown";
                // string characterName = scoreData["characterName"]?.ToString() ?? "Unknown";
                nameText.text = $"{playerName}";
            }

            rank++;
        }
    }
    public void LoadScoresByCharacterName(string characterName = null)
    {
        // Cập nhật tên nhân vật hiện tại
        currentCharacterName = string.IsNullOrEmpty(characterName) ? DEFAULT_CHARACTER : characterName;

        if (btnpanelCharText != null)
        {
            btnpanelCharText.text = currentCharacterName;
        }

        closePanelChar();

        // Tải và cập nhật hình ảnh nhân vật
        Sprite characterSprite = Resources.Load<Sprite>("Title_Character/" + currentCharacterName);
        UpdateCharacterImage(characterSprite);
        LoadScoresForCurrentSelection();
    }

    // Phương thức mới để cập nhật hình ảnh nhân vật
    private void UpdateCharacterImage(Sprite characterSprite)
    {
        if (characterSprite != null)
        {
            characterImage.sprite = characterSprite;
        }
        else
        {
            Debug.LogWarning("Character sprite not found.");
            // Có thể đặt một hình ảnh mặc định ở đây nếu muốn
            // characterImage.sprite = defaultSprite;
        }
    }
    // Xóa các phương thức LoadCharacterSprites() và Dictionary characterSprites

    private void ToggleGameMode()
    {
        // Toggle between SinglePlayer and BotMode
        currentGameMode = currentGameMode == GameMode.SinglePlayer ? 
                         GameMode.BotMode : 
                         GameMode.SinglePlayer;

        UpdateGameModeText();
        LoadScoresForCurrentSelection();
    }

    private void ToggleMap()
    {
        // Cycle through available maps
        int currentIndex = Array.IndexOf(availableMaps, currentMap);
        currentIndex = (currentIndex + 1) % availableMaps.Length;
        currentMap = availableMaps[currentIndex];

        UpdateMapText();
        LoadScoresForCurrentSelection();
    }

    private void UpdateGameModeText()
    {
        if (gameModeText != null)
        {
            switch (currentGameMode)
            {
                case GameMode.SinglePlayer:
                    gameModeText.text = "Single Player";
                    break;
                case GameMode.BotMode:
                    gameModeText.text = "Bot Mode";
                    break;
            }
        }
    }

    private void UpdateMapText()
    {
        if (mapText != null)
        {
            mapText.text = $"Map: {currentMap}";
        }
    }

    private void LoadScoresForCurrentSelection()
    {
        if (isPersonalScores)
        {
            LoadPersonalScoresForMode();
        }
        else
        {
            LoadGlobalScoresForMode();
        }
    }

    private void LoadPersonalScoresForMode()
    {
        if (user == null)
        {
            Debug.LogError("No user signed in to load scores.");
            return;
        }

        string userId = user.UserId;
        string gameModePath = currentGameMode.ToString().ToLower();

        Debug.Log($"Loading personal scores for User: {userId}, Mode: {gameModePath}, Map: {currentMap}");
        
        DatabaseReference scoresRef = dbReference.Child("users")
            .Child(userId)
            .Child("scores")
            .Child(gameModePath)
            .Child(currentMap);

        scoresRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Error retrieving personal scores: {task.Exception}");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log($"Retrieved data exists: {snapshot.Exists}, Child count: {snapshot.ChildrenCount}");
                PopulateScoreList(snapshot, true);
            }
        });
    }

    private void LoadGlobalScoresForMode()
    {
        string gameModePath = currentGameMode.ToString().ToLower();

        Debug.Log($"Loading global scores for Mode: {gameModePath}, Map: {currentMap}");
        
        DatabaseReference scoresRef = dbReference.Child("globalScores")
            .Child(gameModePath)
            .Child(currentMap);

        scoresRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Error retrieving global scores: {task.Exception}");
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log($"Retrieved data exists: {snapshot.Exists}, Child count: {snapshot.ChildrenCount}");
                PopulateScoreList(snapshot, false);
            }
        });
    }

    // Thêm phương thức mới để xử lý việc chuyển đổi
    private void ToggleScoreView()
    {
        isPersonalScores = !isPersonalScores;
        UpdateToggleButtonText();
        LoadScoresForCurrentSelection();
    }

    private void UpdateToggleButtonText()
    {
        if (toggleButtonText != null)
        {
            toggleButtonText.text = isPersonalScores ? "Personal Scores" : "Global Scores";
        }
    }
}
