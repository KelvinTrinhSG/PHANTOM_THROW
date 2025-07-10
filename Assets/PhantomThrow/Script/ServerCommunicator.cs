using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

public class ServerCommunicator : MonoBehaviour
    {
    private string baseUrl = "http://localhost:3000"; // 🔁 Đổi nếu backend chạy ở nơi khác

    public Button restartButton;
    public Button backButton;

    /// <summary>
    /// Gửi username và apple_unwritten lên server để cập nhật nếu cần.
    /// </summary>
    public void UpdateAppleUnwritten(string username, int appleUnwritten)
        {
        StartCoroutine(PostAppleUnwrittenCoroutine(username, appleUnwritten));
        }

    private IEnumerator PostAppleUnwrittenCoroutine(string username, int appleUnwritten)
        {
        string url = baseUrl + "/add-apple-unwritten";

        string jsonBody = JsonUtility.ToJson(new AppleData
            {
            username = username,
            apple_unwritten = appleUnwritten
            });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityWebRequest.Result.Success)
#else
        if (!request.isNetworkError && !request.isHttpError)
#endif
            {
            Debug.Log("✅ Server response: " + request.downloadHandler.text);
            }
        else
            {
            Debug.LogError("❌ Error sending apple_unwritten: " + request.error);
            }
        PlayerSession.Instance.FetchAppleUnwritten();
        }

    /// <summary>
    /// Gọi backend để reset apple_unwritten về 0 cho user.
    /// </summary>
    public void ResetAppleUnwritten(string username)
        {
        StartCoroutine(ResetAppleUnwrittenCoroutine(username));
        }

    private IEnumerator ResetAppleUnwrittenCoroutine(string username)
        {
        string url = baseUrl + "/reset-apple-unwritten";

        string jsonBody = JsonUtility.ToJson(new UsernamePayload { username = username });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityWebRequest.Result.Success)
#else
    if (!request.isNetworkError && !request.isHttpError)
#endif
            {
            Debug.Log("✅ apple_unwritten reset response: " + request.downloadHandler.text);
            PlayerSession.Instance.UpdateAllUI();
            restartButton.interactable = true;
            backButton.interactable = true;
            }
        else
            {
            Debug.LogError("❌ Failed to reset apple_unwritten: " + request.error);
            }
        }

    /// <summary>
    /// Gửi request để cộng thêm apple_written cho user.
    /// </summary>
    public void UpdateAppleWritten(string username, int appleWritten)
        {
        StartCoroutine(UpdateAppleWrittenCoroutine(username, appleWritten));
        }

    private IEnumerator UpdateAppleWrittenCoroutine(string username, int appleWritten)
        {
        string url = baseUrl + "/update-apple-written";

        var jsonBody = $"{{\"username\":\"{username}\",\"apple_written\":{appleWritten}}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
            {
            Debug.LogError("❌ UpdateAppleWritten failed: " + request.error);
            }
        else
            {
            Debug.Log("✅ UpdateAppleWritten response: " + request.downloadHandler.text);
            ResetAppleUnwritten(UserSession.Instance.Username);
            PlayerSession.Instance.UpdateAllUI();
            }
        }

    public void UpdateSwordCollected(string username, int swordCollected)
        {
        StartCoroutine(UpdateSwordCollectedCoroutine(username, swordCollected));
        }

    private IEnumerator UpdateSwordCollectedCoroutine(string username, int swordCollected)
        {
        string url = $"{baseUrl}/update-sword-collected";

        string jsonBody = JsonUtility.ToJson(new SwordCollectedData
            {
            username = username,
            sword_collected = swordCollected
            });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
            {
            Debug.LogError("❌ UpdateSwordCollected failed: " + request.error);
            }
        else
            {
            Debug.Log("✅ UpdateSwordCollected response: " + request.downloadHandler.text);
            }
        PlayerSession.Instance.UpdateAllUI();
        }

    public void IncrementTxCount(string username)
        {
        StartCoroutine(IncrementTxCountCoroutine(username));
        }

    private IEnumerator IncrementTxCountCoroutine(string username)
        {
        string url = $"{baseUrl}/increment-tx-count";

        string jsonBody = JsonUtility.ToJson(new TxCountRequest
            {
            username = username
            });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
    if (request.isNetworkError || request.isHttpError)
#endif
            {
            Debug.LogError("❌ IncrementTxCount failed: " + request.error);
            }
        else
            {
            Debug.Log("✅ IncrementTxCount response: " + request.downloadHandler.text);
            }
        PlayerSession.Instance.UpdateAllUI();
        }

    [System.Serializable]
    private class TxCountRequest
        {
        public string username;
        }

    [System.Serializable]
    private class SwordCollectedData
        {
        public string username;
        public int sword_collected;
        }

    [System.Serializable]
    private class UsernamePayload
        {
        public string username;
        }

    [System.Serializable]
    private class AppleData
        {
        public string username;
        public int apple_unwritten;
        }
    }
