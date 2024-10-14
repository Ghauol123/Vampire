using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // or TMPro for TMP_InputField
using TMPro;

public class TabBetweenFields : MonoBehaviour
{
    // List of input fields for Login Form
    public List<TMP_InputField> loginInputFields;
    
    // List of input fields for Sign-Up Form
    public List<TMP_InputField> signUpInputFields;

    public List<TMP_InputField> ResetPasswordInputFields;

    // Variables to keep track of current index in each form
    private int currentLoginFieldIndex = 0;
    private int currentSignUpFieldIndex = 0;
    private int currentResetPasswordFieldIndex = 0;

    // Boolean to track which form is active (true = Login, false = Sign-Up)
    public bool isLoginActive = true; // Assume login form is active by default
    public bool isResetPasswordActive = false;

    void Start()
    {
        // Ensure the first field in the active form is focused
        if (isLoginActive && loginInputFields.Count > 0)
        {
            currentLoginFieldIndex = 0;
            loginInputFields[currentLoginFieldIndex].ActivateInputField();
        }
        else if (signUpInputFields.Count > 0)
        {
            currentSignUpFieldIndex = 0;
            signUpInputFields[currentSignUpFieldIndex].ActivateInputField();
        }
        else if (ResetPasswordInputFields.Count > 0)
        {
            currentResetPasswordFieldIndex = 0;
            ResetPasswordInputFields[currentResetPasswordFieldIndex].ActivateInputField();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isLoginActive)
            {
                // Move to the next input field in the Login form
                NextLoginInputField();
            }
            else if (!isLoginActive && !isResetPasswordActive)
            {
                // Move to the next input field in the Sign-Up form
                NextSignUpInputField();
            }
            else if (isResetPasswordActive)
            {
                // Move to the next input field in the Reset Password form
                NextResetPasswordInputField();
            }
        }
    }

    void NextLoginInputField()
    {
        if (loginInputFields.Count == 0) return; // Ensure the list isn't empty

        // Deactivate the current input field
        loginInputFields[currentLoginFieldIndex].DeactivateInputField();

        // Move to the next field, loop back if we reach the end
        currentLoginFieldIndex = (currentLoginFieldIndex + 1) % loginInputFields.Count;

        // Activate the next input field
        loginInputFields[currentLoginFieldIndex].ActivateInputField();
    }

    void NextSignUpInputField()
    {
        if (signUpInputFields.Count == 0) return; // Ensure the list isn't empty

        // Deactivate the current input field
        signUpInputFields[currentSignUpFieldIndex].DeactivateInputField();

        // Move to the next field, loop back if we reach the end
        currentSignUpFieldIndex = (currentSignUpFieldIndex + 1) % signUpInputFields.Count;

        // Activate the next input field
        signUpInputFields[currentSignUpFieldIndex].ActivateInputField();
    }

    void NextResetPasswordInputField()
    {
        if (ResetPasswordInputFields.Count == 0) return; // Ensure the list isn't empty

        // Deactivate the current input field
        ResetPasswordInputFields[currentResetPasswordFieldIndex].DeactivateInputField();

        // Move to the next field, loop back if we reach the end
        currentResetPasswordFieldIndex = (currentResetPasswordFieldIndex + 1) % ResetPasswordInputFields.Count;

        // Activate the next input field
        ResetPasswordInputFields[currentResetPasswordFieldIndex].ActivateInputField();
    }
    // Optional method to switch forms (Login <-> Sign Up)
    public void SwitchToLogin()
    {
        isLoginActive = true;

        // Deactivate all Sign-Up fields
        foreach (var field in signUpInputFields)
        {
            field.DeactivateInputField();
        }

        // Activate the first field in the Login form
        if (loginInputFields.Count > 0)
        {
            currentLoginFieldIndex = 0;
            loginInputFields[currentLoginFieldIndex].ActivateInputField();
        }
    }

    public void SwitchToSignUp()
    {
        isLoginActive = false;

        // Deactivate all Login fields
        foreach (var field in loginInputFields)
        {
            field.DeactivateInputField();
        }

        // Activate the first field in the Sign-Up form
        if (signUpInputFields.Count > 0)
        {
            currentSignUpFieldIndex = 0;
            signUpInputFields[currentSignUpFieldIndex].ActivateInputField();
        }
    }
    public void SwitchToResetPassword()
    {
        isResetPasswordActive = true;

        // Deactivate all Login and Sign-Up fields
        foreach (var field in loginInputFields)
        {
            field.DeactivateInputField();   
        }
        foreach (var field in signUpInputFields)
        {
            field.DeactivateInputField();
        }

        // Activate the first field in the Reset Password form
        if (ResetPasswordInputFields.Count > 0)
        {   
            currentResetPasswordFieldIndex = 0;
            ResetPasswordInputFields[currentResetPasswordFieldIndex].ActivateInputField();
        }
    }
}
