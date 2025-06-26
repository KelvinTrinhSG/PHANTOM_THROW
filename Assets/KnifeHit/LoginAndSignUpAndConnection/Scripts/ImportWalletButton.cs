using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class ImportWalletButton : MonoBehaviour
    {
    public TMP_InputField mnemonicsInput;

    public GameObject walletInformationPanel;

    public TextMeshProUGUI walletAddressValue;
    public TextMeshProUGUI publicKeyValue;
    public TextMeshProUGUI privateKeyValue;
    public TextMeshProUGUI mnemonicsValue;
    public TextMeshProUGUI feedbackImportText;

    private string updateWalletUrl;
    private string url;

    private void Start()
        {
        updateWalletUrl = "http://localhost:3000/update-wallet";
        url = "http://localhost:3001/wallet/import"; // ✅ CHỈNH ĐÚNG URL IMPORT
        }

    [ContextMenu("TestImportWallet")]
    public void OnClickImportWallet()
        {
        UpdateFeedbackImportText("");
        string mnemonic = mnemonicsInput.text.Trim();

        if (string.IsNullOrEmpty(mnemonic))
            {
            Debug.LogWarning("❌ Mnemonics is empty!");
            UpdateFeedbackImportText("Mnemonics is empty!");
            return;
            }

        StartCoroutine(CallImportWalletAPI(mnemonic));
        }

    private void UpdateFeedbackImportText(string inputValue)
        {
        feedbackImportText.text = inputValue;
        }

    private void ShowWalletInfo(string address, string publicKey, string privateKey, string mnemonic)
        {
        walletAddressValue.text = address;
        publicKeyValue.text = publicKey;
        privateKeyValue.text = privateKey;
        mnemonicsValue.text = mnemonic;

        walletInformationPanel.SetActive(true);
        }

    IEnumerator CallImportWalletAPI(string mnemonic)
        {

        string jsonBody = JsonUtility.ToJson(new MnemonicRequest { mnemonic = mnemonic });

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            {
            Debug.Log("✅ Wallet imported successfully!");
            Debug.Log("Wallet Info: " + request.downloadHandler.text);

            string json = request.downloadHandler.text;
            WalletResponse wallet = JsonUtility.FromJson<WalletResponseWrapper>("{\"data\":" + json + "}").data;

            ShowWalletInfo(wallet.address, wallet.publicKey, wallet.privateKey, mnemonic);

            UserSession.Instance.SetUserWalletInform(wallet.address, mnemonic);

            Debug.Log(UserSession.Instance.Username);
            Debug.Log(UserSession.Instance.WalletAddress);
            Debug.Log(UserSession.Instance.Mnemonics);

            UpdateWalletToServer();
            }
        else
            {
            Debug.LogError("❌ Failed to import wallet: " + request.error);
            UpdateFeedbackImportText("Input the correct Mnemonics");
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
    class MnemonicRequest
        {
        public string mnemonic;
        }

    [System.Serializable]
    public class WalletResponse
        {
        public string address;
        public string publicKey;
        public string privateKey;
        public string mnemonic;
        }

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

