using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Firebase.Extensions;
using System;
using UnityEngine.SceneManagement;
using Firebase.Database;
using System.Text.RegularExpressions;

public class FirebaseController : MonoBehaviour
{
    public static FirebaseController instance; // Singleton pattern
    private DatabaseReference dbReference; // Database reference

    public GameObject loginPanel, signupPanel, forgotPasswordPanel, notificationPanel;
    // public GameObject gamePanel, profilePanel, UILogin;
    public TMP_InputField emailInput, passwordInput, signupEmailInput, signupPasswordInput, signupNameInput, signupConfirmPasswordInput, forgotEmailInput;
    public TextMeshProUGUI notificationText;
    // public TextMeshProUGUI profileUserName, profileEmail;
    public Toggle rememberMeToggle;
    private FirebaseAuth auth;
    private FirebaseUser user;
    public bool isSignedIn = false;
    // public GameObject scoreScrollViewContent;
    // public GameObject scoreEntryPrefab;
    public string userId;
    TabBetweenFields tabBetweenFields;
    public Canvas mainCanvas; // Reference to the main canvas
    public List<CharacterData> characterDataList; // Use a list to handle multiple characters

    private void Awake()
    {
        tabBetweenFields = FindAnyObjectByType<TabBetweenFields>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Ensure this object persists across scenes
            InitializeFirebase(); // Ensure Firebase is initialized in Awake
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
            return;
        }
    }

    void Start()
    {
        SetCanvasActiveState();
        OpenLoginPanel();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeFirebase(); // Reinitialize Firebase when a new scene is loaded
        SetCanvasActiveState();
        AssignCameraToCanvas(); // Re-assign camera when a new scene is loaded
    }

    private void SetCanvasActiveState()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Login") // Replace "LoginScene" with the actual name of your login scene
        {
            mainCanvas.gameObject.SetActive(true);
        }
        else
        {
            mainCanvas.gameObject.SetActive(false);
        }
    }

    private void AssignCameraToCanvas()
    {
        if (mainCanvas != null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCanvas.worldCamera = mainCamera;
            }
            else
            {
                Debug.LogWarning("Main camera not found. Ensure there is a camera tagged as 'MainCamera' in the scene.");
            }
        }
        else
        {
            Debug.LogError("Main canvas is not assigned.");
        }
    }

public void OpenLoginPanel()
{
    if (loginPanel == null)
    {
        Debug.LogError("Login panel is not assigned.");
        return;
    }
    if (signupPanel == null)
    {
        Debug.LogError("Signup panel is not assigned.");
        return;
    }
    if (forgotPasswordPanel == null)
    {
        Debug.LogError("Forgot password panel is not assigned.");
        return;
    }
    if (tabBetweenFields == null)
    {
        Debug.LogError("TabBetweenFields is not assigned.");
        return;
    }

    loginPanel.SetActive(true);
    signupPanel.SetActive(false);
    tabBetweenFields.isLoginActive = true;
    forgotPasswordPanel.SetActive(false);
    // gamePanel.SetActive(false);
    // profilePanel.SetActive(false);
}

    public void OpenSignupPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        tabBetweenFields.isLoginActive = false;
        forgotPasswordPanel.SetActive(false);
        // gamePanel.SetActive(false);
        // profilePanel.SetActive(false);
    }

    public void OpenProfilePanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        // profilePanel.SetActive(true);
        forgotPasswordPanel.SetActive(false);
    }

    public void OpenForgotPasswordPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        // gamePanel.SetActive(false);
        forgotPasswordPanel.SetActive(true);
        // profilePanel.SetActive(false);
    }

    public void OpenGamePanel()
    {
        // UILogin.SetActive(false);
        // profilePanel.SetActive(false);
        // gamePanel.SetActive(true);
        SceneManager.LoadScene("Main Screen");
    }

    public void LoginUser()
    {
        List<string> errors = new List<string>();

        if (!IsValidEmail(emailInput.text))
            errors.Add("Invalid email format");

        if (!IsValidPassword(passwordInput.text))
            errors.Add("Invalid password format");

        if (errors.Count > 0)
        {
            ShowNotification(errors);
            return;
        }

        SignInUser(emailInput.text, passwordInput.text);
    }

    public void SignUpUser()
    {
        List<string> errors = new List<string>();

        // Kiểm tra các trường cần thiết
        if (string.IsNullOrEmpty(signupEmailInput.text))
            errors.Add("Email is required");
        else if (!IsValidEmail(signupEmailInput.text))
            errors.Add("Invalid email format");

        if (string.IsNullOrEmpty(signupPasswordInput.text))
            errors.Add("Password is required");
        else if (!IsValidPassword(signupPasswordInput.text))
            errors.Add("Password must be at least 6 characters long, start with an uppercase letter, and contain at least one special character");

        if (string.IsNullOrEmpty(signupNameInput.text))
            errors.Add("Name is required");

        if (string.IsNullOrEmpty(signupConfirmPasswordInput.text))
            errors.Add("Confirm Password is required");

        // Kiểm tra xem mật khẩu và xác nhận mật khẩu có khớp không
        if (signupPasswordInput.text != signupConfirmPasswordInput.text)
            errors.Add("Password and Confirm Password do not match");

        // Nếu có lỗi, hiển thị danh sách lỗi và dừng việc xử lý
        if (errors.Count > 0)
        {
            ShowNotification(errors); // Hiển thị tất cả các lỗi
            return;
        }

        // Nếu không có lỗi, tiếp tục tạo người dùng
        CreateUser(signupEmailInput.text, signupPasswordInput.text, signupNameInput.text);
    }

    public void ForgotPassword()
    {
        if (string.IsNullOrEmpty(forgotEmailInput.text))
        {
            ShowNotification(new List<string> { "Email is required" });
            return;
        }
        if (!IsValidEmail(forgotEmailInput.text))
        {
            ShowNotification(new List<string> { "Invalid email format" });
            return;
        }
        // Implement forgot password functionality here
    }

    public void CreateUser(string email, string password, string username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowNotification(new List<string> { "* User creation was canceled." });
                return;
            }
            if (task.IsFaulted)
            {
                List<string> errors = new List<string> { "* User creation encountered an error:" };
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        errors.Add(GetErrorMessageFromException(errorCode));
                    }
                }
                ShowNotification(errors);
                return;
            }
            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            
            // Initialize characters for the new user
            InitializeCharactersForNewUser(newUser.UserId, characterDataList);

            // Send email verification
            SendVerificationEmail(newUser);
        });
    }

    private void InitializeCharactersForNewUser(string userId, List<CharacterData> characterDataList)
    {
        if (dbReference == null)
        {
            Debug.LogError("Database reference is null. Cannot initialize characters.");
            return;
        }

        int initialLevel = 1;

        foreach (CharacterData characterData in characterDataList)
        {
            string characterId = characterData.Name; // Use the character's name as the ID
            string path = $"users/{userId}/characters/{characterId}/level";
            dbReference.Child(path).SetValueAsync(initialLevel).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Initialized {characterId} with level {initialLevel} for user {userId}");
                }
                else
                {
                    Debug.LogError($"Failed to initialize {characterId} for user {userId}: {task.Exception}");
                }
            });
        }
    }

    private void SendVerificationEmail(FirebaseUser user)
    {
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWithOnMainThread(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                // Show notification to user
                ShowNotification(new List<string> { "Verification email sent. Please check your email to verify your account." });
            });
        }
    }

    public void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowNotification(new List<string> { "* Sign-in was canceled." });
                return;
            }
            if (task.IsFaulted)
            {
                List<string> errors = new List<string> { "Sign-in encountered an error:" };
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        errors.Add(GetErrorMessageFromException(errorCode));
                    }
                }
                ShowNotification(errors);
                return;
            }
            
            FirebaseUser user = task.Result.User;
            if (!user.IsEmailVerified)
            {
                ShowNotification(new List<string> { "Please verify your email before signing in." });
                auth.SignOut();
                return;
            }
            
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            userId = user.UserId; // Update userId after successful sign-in
            isSignedIn = true;
            // DataPersistenceManager.instance.OnUserChanged();
            OpenGamePanel();
        });
    }

    void InitializeFirebase()
    {
        if (auth == null)
        {
            auth = FirebaseAuth.DefaultInstance;
        }

        if (auth.CurrentUser != null)
        {
            userId = auth.CurrentUser.UserId;
            Debug.Log(userId);
        }
        else
        {
            Debug.Log("No user is currently signed in.");
        }

        dbReference = FirebaseDatabase.DefaultInstance.RootReference; 
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null); // Call StateChanged to check user state immediately
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth == null)
        {
            Debug.LogError("FirebaseAuth instance is null. Cannot check authentication state.");
            return;
        }

        FirebaseUser currentUser = auth.CurrentUser;
        if (currentUser == null)
        {
            Debug.LogWarning("CurrentUser is null. No user is signed in.");
        }

        if (user != currentUser)
        {
            bool signedIn = user == null && currentUser != null;
            if (user != null && !signedIn)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = currentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId + " " + user.DisplayName);
                userId = user.UserId; // Update userId when a new user signs in
                isSignedIn = true;
                // DataPersistenceManager.instance.OnUserChanged(); // Call OnUserChanged when a user signs in
            }
        }
    }

    public void SignOutUser()
    {
        if (auth != null)
        {
            auth.SignOut();
            user = null;
            userId = null;
            isSignedIn = false;
            Debug.Log("User signed out successfully.");
        }
        else
        {
            Debug.LogError("FirebaseAuth instance is null. Cannot sign out.");
        }
    }

    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }
        auth = null; // Only set to null if you are sure you want to reset it
    }
    bool isSigned = false;
    private void Update()
    {
        if (isSignedIn)
        {
            if(!isSigned)
            {
                isSigned = true;
                OpenGamePanel();
            }
        }
    }

    private static string GetErrorMessageFromException(AuthError authError)
    {
        var message = "";
        switch (authError)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Account Exists With Different Credentials";
                break;
            case AuthError.MissingEmail:
                message = "Missing Email";
                break;
            case AuthError.MissingPassword:
                message = "Missing Password";
                break;
            case AuthError.WrongPassword:
                message = "Wrong Password";
                break;
            case AuthError.InvalidEmail:
                message = "Invalid Email";
                break;
            case AuthError.UserNotFound:
                message = "User Not Found";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Email Already In Use";
                break;
            case AuthError.WeakPassword:
                message = "Weak Password. Password must be at least 6 characters long, start with an uppercase letter, and contain at least one special character.";
                break;
            default:
                message = "Unknown Error";
                break;
        }
        return message;
    }

    void ShowNotification(List<string> errors)
    {
        // Xóa nội dung cũ của thông báo
        notificationText.text = "";

        // Tạo thông báo lỗi với dấu sao và xuống dòng
        string formattedMessage = "";

        foreach (string error in errors)
        {
            formattedMessage += "* " + error + "\n"; // Dấu sao và xuống dòng cho mỗi lỗi
        }

        notificationText.text = formattedMessage; // Gán nội dung mới cho thông báo
        notificationPanel.SetActive(true); // Hiển thị panel thông báo

        // Ẩn thông báo sau một khoảng thời gian (nếu cần)
        StartCoroutine(HideNotificationAfterDelay(3f));
    }

    IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationPanel.SetActive(false);
    }

    // Add this method to validate email format
    private bool IsValidEmail(string email)
    {
        string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
        return Regex.IsMatch(email, emailPattern);
    }

    // Add this method near the top of the class, after the existing IsValidEmail method
    private bool IsValidPassword(string password)
    {
        // Check if password is at least 6 characters long
        if (password.Length < 6)
            return false;

        // Check if password starts with an uppercase letter
        if (!char.IsUpper(password[0]))
            return false;

        // Check if password contains at least one special character
        string specialCharacters = @"!@#$%^&*()_+=-{}[]|\:;""'<>,.?/";
        return password.IndexOfAny(specialCharacters.ToCharArray()) != -1;
    }

    public void SaveCharacterLevel(string characterId, int level)
    {
        if (user == null)
        {
            Debug.LogError("No user is signed in.");
            return;
        }

        string path = $"users/{user.UserId}/characters/{characterId}/level";
        dbReference.Child(path).SetValueAsync(level).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Character {characterId} level saved: {level}");
            }
            else
            {
                Debug.LogError("Failed to save character level: " + task.Exception);
            }
        });
    }

    public void LoadCharacterLevel(string characterId, Action<int> onLevelLoaded)
    {
        if (user == null)
        {
            Debug.LogError("No user is signed in.");
            return;
        }

        string path = $"users/{user.UserId}/characters/{characterId}/level";
        dbReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int level = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 1;
                Debug.Log($"Character {characterId} level loaded: {level}");
                onLevelLoaded(level);
            }
            else
            {
                Debug.LogError("Failed to load character level: " + task.Exception);
            }
        });
    }
}
