using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class CreateWalletButtonHandler : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Gán Text(TMP) ở đây
    private string apiUrl;
    private string updateWalletUrl;
    public GameObject walletInformationPanel;

    public TextMeshProUGUI walletAddressValue;
    public TextMeshProUGUI publicKeyValue;
    public TextMeshProUGUI privateKeyValue;
    public TextMeshProUGUI mnemonicsValue;

    private void Start()
    {
        // Load từ .env hoặc chỉnh tay tại đây
        //apiUrl = "http://localhost:3001/wallet/create";
        apiUrl = APIEndpoints.CreateWallet;
        //updateWalletUrl = "http://localhost:3000/update-wallet";
        updateWalletUrl = APIEndpoints.UpdateWallet;
        }

    public void CreateWallet()
    {
        if (feedbackText != null)
            feedbackText.text = "Creating wallet...";

        StartCoroutine(CallCreateWallet());
    }

    private void ShowWalletInfo(string address, string publicKey, string privateKey, string mnemonic)
    {
        // Set text
        walletAddressValue.text = address;
        publicKeyValue.text = publicKey;
        privateKeyValue.text = privateKey;
        mnemonicsValue.text = mnemonic;

        // Hiện panel
        walletInformationPanel.SetActive(true);
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

            ShowWalletInfo(
    wallet.address,
    wallet.publicKey,
    wallet.privateKey,
    wallet.mnemonic
);

            // Debug log từng trường
            Debug.Log("📬 Address: " + wallet.address);
            Debug.Log("🪪 Public Key: " + wallet.publicKey);
            Debug.Log("🔐 Private Key: " + wallet.privateKey);
            Debug.Log("🧠 Mnemonic: " + wallet.mnemonic);

            UserSession.Instance.SetUserWalletInform(wallet.address, wallet.mnemonic);

            Debug.Log(UserSession.Instance.Username);
            Debug.Log(UserSession.Instance.WalletAddress);
            Debug.Log(UserSession.Instance.Mnemonics);

            UpdateWalletToServer();


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

    [ContextMenu("Test Update Wallet")]
    public void UpdateWalletToServer()
        {
        StartCoroutine(CallUpdateWalletAPI());
        }

    private IEnumerator CallUpdateWalletAPI()
        {
        // Lấy thông tin từ singleton
        string username = UserSession.Instance.Username;
        string walletAddress = UserSession.Instance.WalletAddress;
        string mnemonics = UserSession.Instance.Mnemonics;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(walletAddress) || string.IsNullOrEmpty(mnemonics))
            {
            Debug.LogWarning("❌ Missing user session data.");
            yield break;
            }

        // Tạo object request
        WalletUpdateRequest payload = new WalletUpdateRequest
            {
            username = username,
            walletaddress = walletAddress,
            mnemonics = mnemonics
            };

        string jsonBody = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(updateWalletUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            {
            Debug.Log("✅ Wallet updated successfully: " + request.downloadHandler.text);
            }
        else
            {
            Debug.LogError("❌ Failed to update wallet: " + request.error);
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

    [System.Serializable]
    public class WalletUpdateRequest
        {
        public string username;
        public string walletaddress;
        public string mnemonics;
        }
    }
