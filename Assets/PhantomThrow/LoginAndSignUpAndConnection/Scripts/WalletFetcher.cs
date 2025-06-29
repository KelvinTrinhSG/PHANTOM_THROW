using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class WalletFetcher : MonoBehaviour
{
    private string getWalletUrl = "http://localhost:3000/get-wallet"; // 🛠 Cập nhật URL nếu cần

    public void FetchWalletForUser(string username)
    {
        StartCoroutine(FetchWalletCoroutine(username));
    }

    IEnumerator FetchWalletCoroutine(string username)
    {
        string url = $"{getWalletUrl}?username={UnityWebRequest.EscapeURL(username)}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text.Contains("walletaddress"))
            {
                WalletResponse response = JsonUtility.FromJson<WalletResponse>(request.downloadHandler.text);

                if (!string.IsNullOrEmpty(response.walletaddress))
                {
                    // ✅ Ví đã tồn tại → lưu vào singleton
                    UserSession.Instance.SetUserData(response.username, response.walletaddress, response.mnemonics, UserSession.Instance.Score);
                    SceneManager.LoadScene("WalletInform");
                    yield break;
                }
            }

            // ❌ Không có ví → chuyển sang WalletManager
            SceneManager.LoadScene("WalletManager");
        }
        else
        {
            Debug.LogError("Failed to fetch wallet: " + request.downloadHandler.text);
            SceneManager.LoadScene("WalletManager");
        }
    }

    [System.Serializable]
    public class WalletResponse
    {
        public string username;
        public string walletaddress;
        public string mnemonics;
    }
}
