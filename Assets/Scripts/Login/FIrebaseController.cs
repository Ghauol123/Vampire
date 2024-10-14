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
    public GameObject scoreScrollViewContent;
    public GameObject scoreEntryPrefab;
    private string userId;
    TabBetweenFields tabBetweenFields;
    private void Awake() {
        tabBetweenFields = FindAnyObjectByType<TabBetweenFields>();
    }
    void Start()
    {
        // if (instance == null)
        // {
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);  // Đảm bảo không bị phá hủy khi chuyển cảnh
        // }
        // else
        // {
        //     Destroy(gameObject);  // Nếu đã có instance, hủy đối tượng mới
        // }
        // Initialize the login button state
        OpenLoginPanel();
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    public void OpenLoginPanel()
    {
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
            
            // Send email verification
            SendVerificationEmail(newUser);
        });
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
            isSignedIn = true;
            DataPersistenceManager.instance.OnUserChanged();
            OpenGamePanel();
        });
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        Debug.Log(userId);
        dbReference = FirebaseDatabase.DefaultInstance.RootReference; 
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth == null)
        {
            Debug.LogError("FirebaseAuth instance is null. Cannot check authentication state.");
            return;
        }

        FirebaseUser currentUser = auth.CurrentUser;
        // if (currentUser == null)
        // {
        //     Debug.LogError("CurrentUser is null. No user is signed in.");
        // }

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
                isSignedIn = true;
                DataPersistenceManager.instance.OnUserChanged(); // Gọi OnUserChanged khi có người dùng đăng nhập
            }
        }
    }

    private void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    // void UpdateUserProfile(string username)
    // {
    //     FirebaseUser currentUser = auth.CurrentUser;
    //     if (currentUser != null)
    //     {
    //         UserProfile profile = new UserProfile
    //         {
    //             DisplayName = username,
    //             PhotoUrl = new System.Uri("https://placehold.co/150x150")
    //         };
    //         currentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
    //         {
    //             if (task.IsCanceled)
    //             {
    //                 Debug.LogError("UpdateUserProfileAsync was canceled.");
    //                 return;
    //             }
    //             if (task.IsFaulted)
    //             {
    //                 Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
    //                 foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
    //                 {
    //                     Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
    //                     if (firebaseEx != null)
    //                     {
    //                         AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
    //                         ShowNotification(GetErrorMessageFromException(errorCode));
    //                     }
    //                 }
    //                 return;
    //             }
    //             Debug.Log("User profile updated successfully.");
    //             OpenLoginPanel();
    //         });
    //     }
    // }

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
}