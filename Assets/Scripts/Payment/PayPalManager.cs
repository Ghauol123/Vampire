using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PayPalManager : MonoBehaviour
{
    private string clientId = "ATojUwNIDWzn3XXSi9eNK5_TXV3-hbm_R1c2x_uyJEwBqEt0DyoQF62ojRCfB4jEd_hrrM_5_oFqfoN3";
    private string secretKey = "ELy_ZUPD0G_af29vVZ93kQiTSoUNkCrqDt_ZsWCLIA9ow_fii7dhfwL66Tj6ewkIYCAcqiHwJN0eGjDT";
    private string apiUrl = "https://api.sandbox.paypal.com/v1/payments/payment"; // Sandbox URL
    private string tokenEndpoint = "https://api.sandbox.paypal.com/v1/oauth2/token"; // Token endpoint for sandbox

    void Start()
    {
        StartCoroutine(MakePaymentRequest());
    }

    IEnumerator MakePaymentRequest()
    {
        // Declare a variable to hold the access token
        string accessToken = null;

        // Call GetAccessToken and wait for it to complete
        yield return StartCoroutine(GetAccessToken(token => accessToken = token));

        if (!string.IsNullOrEmpty(accessToken))
        {
            // Create the payment request body (this should be structured according to PayPal's API documentation)
            string paymentRequestBody = @"{
                ""intent"": ""sale"",
                ""redirect_urls"": {
                    ""return_url"": ""https://example.com/success"",
                    ""cancel_url"": ""https://example.com/cancel""
                },
                ""payer"": {
                    ""payment_method"": ""paypal""
                },
                ""transactions"": [
                    {
                        ""amount"": {
                            ""total"": ""10.00"",
                            ""currency"": ""USD""
                        },
                        ""description"": ""Payment description""
                    }
                ]
            }";

            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(paymentRequestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + accessToken);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error in payment request: " + request.error);
            }
            else
            {
                // Parse the payment response to get the approval URL
                PaymentResponse paymentResponse = JsonUtility.FromJson<PaymentResponse>(request.downloadHandler.text);
                string approvalUrl = paymentResponse.links.Find(link => link.rel == "approval_url")?.href;

                if (!string.IsNullOrEmpty(approvalUrl))
                {
                    // Redirect the user to the PayPal approval URL
                    Application.OpenURL(approvalUrl);
                }
                else
                {
                    Debug.LogError("Failed to obtain PayPal approval URL");
                }
            }
        }
        else
        {
            Debug.LogError("Failed to obtain access token");
        }
    }

    IEnumerator GetAccessToken(System.Action<string> callback)
    {
        string authString = clientId + ":" + secretKey;
        string base64Auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(authString));

        UnityWebRequest request = UnityWebRequest.Post(tokenEndpoint, "grant_type=client_credentials");
        request.SetRequestHeader("Authorization", "Basic " + base64Auth);
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error obtaining access token: " + request.error);
            callback(null); // Return null in case of an error
        }
        else
        {
            // Parse and return the access token from the response
            AccessTokenResponse response = JsonUtility.FromJson<AccessTokenResponse>(request.downloadHandler.text);
            callback(response.access_token); // Call the callback with the access token
        }
    }

    [System.Serializable]
    private class AccessTokenResponse
    {
        public string access_token;
        // Add additional fields if present in the actual response
    }

    [System.Serializable]
    private class PaymentResponse
    {
        public List<Link> links;

        [System.Serializable]
        public class Link
        {
            public string href;
            public string rel;
        }
    }
}
