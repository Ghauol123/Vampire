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
    private bool isPersonalScores = true;     // Biến để theo dõi trạng thái hiện tại

    void Start()
    {
        // Khởi tạo Firebase
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        // Gán sự kiện OnClick cho nút toggle
        toggleButton.onClick.AddListener(ToggleScores);

        // Tải điểm cá nhân ban đầu
        LoadPersonalScores();
    }

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
            // Tải điểm cá nhân
            LoadPersonalScores();
        }
        else
        {
            // Tải điểm toàn cầu
            LoadGlobalScores();
        }
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
                DisplayScores(snapshot, isPersonalScores: true);
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
                DisplayScores(snapshot, isPersonalScores: false);
            }
        });
    }

    // Phương thức hiển thị điểm từ snapshot, thay đổi tùy thuộc vào việc là điểm cá nhân hay toàn cầu
    private void DisplayScores(DataSnapshot snapshot, bool isPersonalScores)
    {
        List<Dictionary<string, object>> scoreList = new List<Dictionary<string, object>>();

        foreach (DataSnapshot scoreSnapshot in snapshot.Children)
        {
            Dictionary<string, object> scoreData = (Dictionary<string, object>)scoreSnapshot.Value;
            scoreList.Add(scoreData);
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
                nameText.text =scoreData["characterName"].ToString();
            }
            else
            {
                nameText.text = scoreData["playerName"].ToString(); // Đảm bảo bạn lưu "playerName" khi lưu điểm global
            }

            rank++;
        }
    }
}
