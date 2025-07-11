using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text feedbackText;

    //private string loginUrl = "http://localhost:3000/login";
    private string loginUrl = APIEndpoints.Login;

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    void OnLoginClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        feedbackText.text = "";
        feedbackText.text = "Logging in...";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please enter both username and password.";
            return;
        }

        StartCoroutine(SendLoginRequest(username, password));
    }

    IEnumerator SendLoginRequest(string username, string password)
    {
        string jsonBody = JsonUtility.ToJson(new LoginRequest(username, password));

        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text.Contains("Login successful"))
            {
                feedbackText.text = "Login successful.";
                Debug.Log("Login success: " + request.downloadHandler.text);

                // ✅ Parse username từ request hoặc response (nếu cần)
                UserSession.Instance.SetUserData(username, "", "", 0); // Tạm thời chưa có wallet/mnemonics/score

                // ✅ Gọi WalletFetcher để kiểm tra ví
                WalletFetcher fetcher = FindObjectOfType<WalletFetcher>();
                if (fetcher != null)
                {
                    fetcher.FetchWalletForUser(username);
                }
                else
                {
                    Debug.LogError("WalletFetcher not found in scene.");
                }
            }
            else
            {
                feedbackText.text = "Login failed. " + request.downloadHandler.text;
            }
        }
        else
        {
            feedbackText.text = "Login failed. " + request.downloadHandler.text;
            Debug.LogError("Login error: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;

        public LoginRequest(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
