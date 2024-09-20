using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginBtn : MonoBehaviour
{
    public Button loginButton;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLoginButtonState();
        FirebaseController.instance.emailInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });
        FirebaseController.instance.passwordInput.onValueChanged.AddListener(delegate { UpdateLoginButtonState(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateLoginButtonState()
    {
        if (string.IsNullOrEmpty(FirebaseController.instance.emailInput.text) || string.IsNullOrEmpty(FirebaseController.instance.passwordInput.text))
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
}
