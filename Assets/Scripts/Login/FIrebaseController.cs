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

public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, signupPanel, forgotPasswordPanel, notificationPanel;
    // public GameObject gamePanel, profilePanel, UILogin;
    public TMP_InputField emailInput, passwordInput, signupEmailInput, signupPasswordInput, signupNameInput, signupConfirmPasswordInput, forgotEmailInput;
    public Button loginButton;
    public TextMeshProUGUI notificationText;
    // public TextMeshProUGUI profileUserName, profileEmail;
    public Toggle rememberMeToggle;
    private FirebaseAuth auth;
    private FirebaseUser user;
    public bool isSignedIn = false;

    void Start()
    {
        emailInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });
        passwordInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });

        // Initialize the login button state
        UpdateLoginButtonState();
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

    void UpdateLoginButtonState()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            // Gray color if input is invalid
            loginButton.interactable = false;
            loginButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            // Red color if input is valid
            loginButton.interactable = true;
            loginButton.GetComponent<Image>().color = Color.red;
        }
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
        // gamePanel.SetActive(false);
        // profilePanel.SetActive(false);
    }

    public void OpenSignupPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
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
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.LogError("Email or Password is empty");
            return;
        }
        SignInUser(emailInput.text, passwordInput.text);
    }

    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signupEmailInput.text) || string.IsNullOrEmpty(signupPasswordInput.text) || string.IsNullOrEmpty(signupNameInput.text) || string.IsNullOrEmpty(signupConfirmPasswordInput.text))
        {
            string missingFields = "";
            if (string.IsNullOrEmpty(signupEmailInput.text))
                missingFields += "Email, ";
            if (string.IsNullOrEmpty(signupPasswordInput.text))
                missingFields += "Password, ";
            if (string.IsNullOrEmpty(signupNameInput.text))
                missingFields += "Name, ";
            if (string.IsNullOrEmpty(signupConfirmPasswordInput.text))
                missingFields += "Confirm Password, ";

            // Remove the last comma and space
            if (missingFields.Length > 0)
                missingFields = missingFields.Substring(0, missingFields.Length - 2);

            ShowNotification("Missing: " + missingFields);
            return;
        }

        if (signupPasswordInput.text != signupConfirmPasswordInput.text)
        {
            Debug.LogError("Password and Confirm Password does not match");
            return;
        }
        CreateUser(signupEmailInput.text, signupPasswordInput.text, signupNameInput.text);
    }

    public void ForgotPassword()
    {
        if (string.IsNullOrEmpty(forgotEmailInput.text))
        {
            Debug.LogError("Email is empty");
            return;
        }
    }

    public void CreateUser(string email, string password, string username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotification(GetErrorMessageFromException(errorCode));
                    }
                }
                return;
            }
            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            UpdateUserProfile(username);
        });
    }

    public void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotification(GetErrorMessageFromException(errorCode));
                    }
                }
                return;
            }
            // On successful sign-in, update UI and switch to the game screen
            FirebaseUser user = task.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            // UpdateUserInformation(user);
            OpenGamePanel(); // Switch to the game panel
        });
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
{
    FirebaseUser currentUser = auth.CurrentUser;
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
            Debug.Log("Signed in " + user.UserId);
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

    void UpdateUserProfile(string username)
    {
        FirebaseUser currentUser = auth.CurrentUser;
        if (currentUser != null)
        {
            UserProfile profile = new UserProfile
            {
                DisplayName = username,
                PhotoUrl = new System.Uri("https://placehold.co/150x150")
            };
            currentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                    {
                        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                        if (firebaseEx != null)
                        {
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            ShowNotification(GetErrorMessageFromException(errorCode));
                        }
                    }
                    return;
                }
                Debug.Log("User profile updated successfully.");
                OpenLoginPanel();
            });
        }
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
                message = "Weak Password";
                break;
            default:
                message = "Unknown Error";
                break;
        }
        return message;
    }

    void ShowNotification(string message)
    {
        // Show a notification with the provided message
        notificationText.text = message;
        notificationPanel.SetActive(true);
        // Optionally, hide the notification after some time
        StartCoroutine(HideNotificationAfterDelay(3f));
    }

    IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationPanel.SetActive(false);
    }

    // void UpdateUserInformation(FirebaseUser user)
    // {
    //     // Update the UI elements with the user's information
    //     if (user != null)
    //     {
    //         profileUserName.text = "Username: " + (user.DisplayName ?? "N/A");
    //         profileEmail.text = "Email: " + user.Email;
    //     }
    // }
}
