using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CreateWalletButtonHandler : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Gán Text(TMP) ở đây
    private string apiUrl;

    private void Start()
    {
        // Load từ .env hoặc chỉnh tay tại đây
        apiUrl = "http://localhost:3001/wallet/create";
    }

    public void CreateWallet()
    {
        if (feedbackText != null)
            feedbackText.text = "Creating wallet...";

        StartCoroutine(CallCreateWallet());
    }

    private IEnumerator CallCreateWallet()
    {
        UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, "");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("✅ Wallet Created Raw: " + json);

            // Parse JSON
            WalletResponse wallet = JsonUtility.FromJson<WalletResponseWrapper>("{\"data\":" + json + "}").data;

            // Debug log từng trường
            Debug.Log("📬 Address: " + wallet.address);
            Debug.Log("🪪 Public Key: " + wallet.publicKey);
            Debug.Log("🔐 Private Key: " + wallet.privateKey);
            Debug.Log("🧠 Mnemonic: " + wallet.mnemonic);

            if (feedbackText != null)
                feedbackText.text = "Wallet created:\n" + wallet.address;
        }
        else
        {
            Debug.LogError("❌ Wallet creation failed: " + request.error);
            if (feedbackText != null)
                feedbackText.text = "Wallet creation failed!";
        }
    }

    [System.Serializable]
    public class WalletResponse
    {
        public string address;
        public string publicKey;
        public string privateKey;
        public string mnemonic;
    }

    // Trick để JsonUtility hiểu JSON gốc không có root, nên bọc tạm lại
    [System.Serializable]
    public class WalletResponseWrapper
    {
        public WalletResponse data;
    }
}
