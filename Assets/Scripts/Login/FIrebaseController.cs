using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FIrebaseController : MonoBehaviour
{
    public GameObject loginPanel, signupPanel, profilePanel;
    public TMP_InputField emailInput, passwordInput, signupEmailInput, signupPasswordInput, signupNameInput, signupConfirmPasswordInput;
    public Button loginButton;
      void Start()
    {
        emailInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });
        passwordInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });

        // Khởi tạo trạng thái nút đăng nhập ngay từ đầu
        UpdateLoginButtonState();
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
        profilePanel.SetActive(false);
    }
    public void OpenSignupPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        profilePanel.SetActive(false);
    }
    public void OpenProfilePanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        profilePanel.SetActive(true);
    }
    public void LoginUser(){
        if(string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text)){
            Debug.LogError("Email or Password is empty");
            return;
        }
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
    }
}
