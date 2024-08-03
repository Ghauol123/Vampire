using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Firebase.Extensions;


public class FirebaseController : MonoBehaviour
{
    public GameObject loginPanel, signupPanel, forgotPasswordPanel, gamePanel, profilePanel, UILogin;
    public TMP_InputField emailInput, passwordInput, signupEmailInput, signupPasswordInput, signupNameInput, signupConfirmPasswordInput, forgotEmailInput, profileUserName, profileEmail;
    public Button loginButton;
    public Toggle rememberMeToggle;
    private FirebaseAuth auth;
    private FirebaseUser user;
    bool isSignedIn = false;
      void Start()
    {
        emailInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });
        passwordInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });

        // Khởi tạo trạng thái nút đăng nhập ngay từ đầu
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
        // Màu xám nếu thông tin nhập vào không hợp lệ
        loginButton.interactable = false;
        loginButton.GetComponent<Image>().color = Color.gray;
    }
    else
    {
        // Màu đỏ nếu thông tin nhập vào hợp lệ
        loginButton.interactable = true;
        loginButton.GetComponent<Image>().color = Color.red;
    }
}
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        forgotPasswordPanel.SetActive(false);
        gamePanel.SetActive(false);
        profilePanel.SetActive(false);
    }
    public void OpenSignupPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        forgotPasswordPanel.SetActive(false);
        gamePanel.SetActive(false);
        profilePanel.SetActive(false);
    }
    public void OpenProfilePanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
        forgotPasswordPanel.SetActive(false);
        // gamePanel.SetActive(false);
    }
    public void OpenForgotPasswordPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        gamePanel.SetActive(false);
        forgotPasswordPanel.SetActive(true);
        profilePanel.SetActive(false);
    }
    public void OpenGamePanel()
    {
        UILogin.SetActive(false);
        gamePanel.SetActive(true);
    }
    public void LoginUser(){
        if(string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text)){
            Debug.LogError("Email or Password is empty");
            return;
        }
        SignInUser(emailInput.text, passwordInput.text);
        OpenGamePanel();
    }
    public void SignUpUser(){
        if(string.IsNullOrEmpty(signupEmailInput.text) || string.IsNullOrEmpty(signupPasswordInput.text) || string.IsNullOrEmpty(signupNameInput.text) || string.IsNullOrEmpty(signupConfirmPasswordInput.text)){
            Debug.LogError("Email, Password, Name or Confirm Password is empty");
            return;
        }
        if(signupPasswordInput.text != signupConfirmPasswordInput.text){
            Debug.LogError("Password and Confirm Password does not match");
            return;
        }
        CreateUser(signupEmailInput.text, signupPasswordInput.text, signupNameInput.text);
    }
    public void ForgotPassword(){
        if(string.IsNullOrEmpty(forgotEmailInput.text)){
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
            Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
            return;
        }
        if (task.IsFaulted)
        {
            Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
            return;
        }
        FirebaseUser user = task.Result.User;
        Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        profileUserName.text = "Username: " + user.DisplayName;
        profileEmail.text = "Email: " + user.Email;
        OpenGamePanel();
    });
}

    void InitializeFirebase(){
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
        }
    }
}

    private void OnDestroy() {
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
                return;
            }
            Debug.Log("User profile updated successfully.");
            OpenLoginPanel();
        });
    }
}

    bool isSigned = false;
    private void Update() {
        if(isSignedIn){
            isSigned = true;
            // profileUserName.text = "Username: " + user.DisplayName;
            // profileEmail.text = "Email: " + user.Email;
        }
    }
}
