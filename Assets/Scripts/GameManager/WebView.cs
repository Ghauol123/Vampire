using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebView : MonoBehaviour
{
    public void OnPaymentButtonClick()
{
    string paymentUrl = "http://localhost:3000/";

    // Mở WebView
    Application.OpenURL(paymentUrl);  // Đây chỉ là một ví dụ cơ bản, có thể cần plugin nếu bạn muốn có WebView trực tiếp trong Unity.
}
}
