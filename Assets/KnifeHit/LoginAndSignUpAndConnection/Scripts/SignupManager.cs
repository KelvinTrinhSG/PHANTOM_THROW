using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class SignupManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button signupButton;
    public TMP_Text feedbackText; // Gắn FeedbackText vào đây

    private string signupUrl = "http://localhost:3000/insert"; // 🔁 Đổi nếu cần

    void Start()
    {
        signupButton.onClick.AddListener(OnSignupClicked);
    }

    void OnSignupClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        feedbackText.text = "";
        feedbackText.text = "Signing up...";

        // ✅ Kiểm tra điều kiện tối thiểu
        if (username.Length < 4)
        {
            feedbackText.text = "Username must be at least 4 characters.";
            return;
        }

        if (password.Length < 6)
        {
            feedbackText.text = "Password must be at least 6 characters.";
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))
        {
            feedbackText.text = "Username can only contain letters and numbers.";
            return;
        }

        StartCoroutine(SendSignupRequest(username, password));
    }


    IEnumerator SendSignupRequest(string username, string password)
    {
        string jsonBody = JsonUtility.ToJson(new SignupRequest(username, password));

        UnityWebRequest request = new UnityWebRequest(signupUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text.Contains("User inserted successfully"))
            {
                feedbackText.text = "Signup successful.";
                Debug.Log("✅ Signup success: " + request.downloadHandler.text);
                // TODO: có thể chuyển scene hoặc auto login
            }
            else
            {
                feedbackText.text = "Signup failed. " + request.downloadHandler.text;
            }
        }
        else
        {
            feedbackText.text = "❌ " + request.downloadHandler.text;
            Debug.LogError("❌ Signup failed: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class SignupRequest
    {
        public string username;
        public string password;

        public SignupRequest(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
