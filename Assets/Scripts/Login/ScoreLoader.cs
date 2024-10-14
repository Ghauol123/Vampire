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

    private const string DEFAULT_CHARACTER = "Amelia";

    void Start()
    {
        // Khởi tạo Firebase
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        // Gán sự kiện OnClick cho nút toggle
        toggleButton.onClick.AddListener(ToggleScores);

        // // Gán sự kiện OnValueChanged cho thanh tìm kiếm
        // searchInputField.onValueChanged.AddListener(OnSearchValueChanged);

        // // Ẩn thanh tìm kiếm ban đầu
        // searchInputField.gameObject.SetActive(false);

        // Tải điểm cá nhân ban đầu với nhân vật mặc định
        LoadScoresByCharacterName(DEFAULT_CHARACTER);
        panelChar.SetActive(false);

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
        btnpanelChar.onClick.AddListener(ToggleCharacterPanel);    }

    // Phương thức chuyển đổi giữa điểm cá nhân và điểm toàn cầu
    void ToggleScores()
    {
        isPersonalScores = !isPersonalScores;

        // Xóa nội dung cũ trước khi tải điểm mới
        foreach (Transform child in scoreScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        if (isPersonalScores)
        {
            // Ẩn thanh tìm kiếm khi hiển thị điểm cá nhân
            // searchInputField.gameObject.SetActive(false);

            // Tải điểm cá nhân
            LoadPersonalScores();
        }
        else
        {
            // Hiển thị thanh tìm kiếm khi hiển thị điểm toàn cầu
            // searchInputField.gameObject.SetActive(true);

            // Tải điểm toàn cầu
            LoadGlobalScores();
        }
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

    // Phương thức tìm kiếm khi có thay đổi trong InputField
    void OnSearchValueChanged(string searchQuery)
    {
        // Lọc danh sách điểm theo tên người chơi
        List<Dictionary<string, object>> filteredScores = allScores.FindAll(score =>
        {
            if (isPersonalScores)
            {
                return score["characterName"].ToString().ToLower().Contains(searchQuery.ToLower());
            }
            else
            {
                return score["playerName"].ToString().ToLower().Contains(searchQuery.ToLower());
            }
        });

        // Hiển thị danh sách đã lọc
        DisplayScores(filteredScores);
    }

    // Phương thức tải điểm cá nhân từ Firebase
    public void LoadPersonalScores()
    {
        if (user == null)
        {
            Debug.LogError("No user signed in to load scores.");
            return;
        }

        string userId = user.UserId;

        DatabaseReference scoresRef = dbReference.Child("users").Child(userId).Child("scores");

        scoresRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving personal scores: " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                PopulateScoreList(snapshot, isPersonalScores: true);
                
                // Sau khi load xong, gọi LoadScoresByCharacterName với nhân vật mặc định
                LoadScoresByCharacterName(DEFAULT_CHARACTER);
            }
        });
    }

    // Phương thức tải điểm toàn cầu từ Firebase
    public void LoadGlobalScores()
    {
        DatabaseReference globalScoresRef = dbReference.Child("globalScores");

        globalScoresRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving global scores: " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                PopulateScoreList(snapshot, isPersonalScores: false);
                
                // Sau khi load xong, gọi LoadScoresByCharacterName với nhân vật mặc định
                LoadScoresByCharacterName(DEFAULT_CHARACTER);
            }
        });
    }

    // Phương thức lưu tất cả điểm vào danh sách và hiển thị
    private void PopulateScoreList(DataSnapshot snapshot, bool isPersonalScores)
    {
        allScores.Clear();

        foreach (DataSnapshot scoreSnapshot in snapshot.Children)
        {
            Dictionary<string, object> scoreData = (Dictionary<string, object>)scoreSnapshot.Value;
            allScores.Add(scoreData);
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

            // Nếu là điểm toàn cầu, hiển thị tên người chơi, nếu không thì hiển thị tên nhân vật
            if (isPersonalScores)
            {
                nameText.text = scoreData["characterName"].ToString();
            }
            else
            {
                nameText.text = scoreData["playerName"].ToString(); // Đảm bảo bạn lưu "playerName" khi lưu điểm global
            }

            rank++;
        }
    }
    public void LoadScoresByCharacterName(string characterName = null)
    {
        // Sử dụng DEFAULT_CHARACTER nếu characterName là null hoặc rỗng
        characterName = string.IsNullOrEmpty(characterName) ? DEFAULT_CHARACTER : characterName;

        // Update the button text when a character is selected
        if (btnpanelCharText != null)
        {
            btnpanelCharText.text = characterName;
        }
        else
        {
            Debug.LogWarning("btnpanelCharText is null. Make sure it's properly assigned in the Inspector.");
        }

        // Close the character panel
        closePanelChar();

        DatabaseReference scoresRef;

        if (isPersonalScores)
        {
            if (user == null)
            {
                Debug.LogError("No user signed in to load scores.");
                return;
            }

            // Load personal scores
            string userId = user.UserId;
            scoresRef = dbReference.Child("users").Child(userId).Child("scores");
        }
        else
        {
            // Load global scores
            scoresRef = dbReference.Child("globalScores");
        }

        scoresRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving scores: " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<Dictionary<string, object>> characterScores = new List<Dictionary<string, object>>();

                foreach (DataSnapshot scoreSnapshot in snapshot.Children)
                {
                    Dictionary<string, object> scoreData = (Dictionary<string, object>)scoreSnapshot.Value;

                    // Filter by character name for both personal and global scores
                    if (scoreData.ContainsKey("characterName") &&
                        scoreData["characterName"].ToString().ToLower() == characterName.ToLower())
                    {
                        characterScores.Add(scoreData);
                    }
                }

                // Display the scores for the given character
                DisplayScores(characterScores);
            }
        });

        // Tải và cập nhật hình ảnh nhân vật
        Sprite characterSprite = Resources.Load<Sprite>("Title_Character/" + characterName);
        UpdateCharacterImage(characterSprite);
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
}
