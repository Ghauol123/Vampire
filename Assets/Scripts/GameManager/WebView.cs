using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebView : MonoBehaviour
{
    public void OnPaymentButtonClick()
{
    string paymentUrl = "https://github.com/Ghauol123/Vampire/blob/master/Assets/Scripts/Login/FIrebaseController.cs";

    // Mở WebView
    Application.OpenURL(paymentUrl);  // Đây chỉ là một ví dụ cơ bản, có thể cần plugin nếu bạn muốn có WebView trực tiếp trong Unity.
}
}
