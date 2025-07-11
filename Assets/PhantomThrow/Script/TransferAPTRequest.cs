using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TransferAPTRequest : MonoBehaviour
    {
    [Header("UI Output")]
    public Text resultText1;
    public Text resultText2;

    //private string apiUrl = "http://localhost:3001/wallet/transfer";
    private string apiUrl = APIEndpoints.WalletTransfer;
    private string receiverAddress = "0x068f5e7020bf02c7cd064e6ab3c9eeb5451d55a26407f3d066b71fbe65f36db7";

    public Button noThanks;
    public Button backButton;
    public Button restartBtn;

    private void Start()
        {
        resultText1.text = "";
        resultText2.text = "";

        noThanks.interactable = true;
        backButton.interactable = true;
        restartBtn.interactable = true;
        }

    public void Send0Point02APT()
        {
        noThanks.interactable = false;
        backButton.interactable = false;
        restartBtn.interactable = false;
        resultText1.text = "Buying...";
        TriggerTransfer(UserSession.Instance.Mnemonics, receiverAddress, 0.02m, 1);
        }

    public void Send0Point1APT()
        {
        noThanks.interactable = false;
        backButton.interactable = false;
        restartBtn.interactable = false;
        resultText2.text = "Buying...";
        TriggerTransfer(UserSession.Instance.Mnemonics, receiverAddress, 0.1m, 2);
        }


    /// <summary>
    /// Gọi hàm này để chuyển APT: amount tính theo APT (ví dụ 0.5)
    /// </summary>
    public void TriggerTransfer(string mnemonic, string receiverAddress, decimal amountInAPT, int indexValue)
        {
        // Chuyển APT → Octas
        long amountInOctas = (long)(amountInAPT * 100_000_000m);
        StartCoroutine(SendTransferRequest(mnemonic, receiverAddress, amountInOctas, indexValue));
        }

    IEnumerator SendTransferRequest(string mnemonic, string to, long amount, int indexValue)
        {
        var payload = new TransferRequestData
            {
            mnemonic = mnemonic,
            to = to,
            amount = amount.ToString() // convert sang string để JSON không lỗi
            };

        string jsonData = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            {
            Debug.LogError("❌ Transfer failed: " + request.error);
            if (indexValue == 1)
                {
                if (resultText1 != null)
                    resultText1.text = "Not Enough APT";
                noThanks.interactable = true;
                backButton.interactable = true;
                restartBtn.interactable = true;
                }
            else {
                if (resultText2 != null)
                    resultText2.text = "Not Enough APT";
                noThanks.interactable = true;
                backButton.interactable = true;
                restartBtn.interactable = true;
                }
            }
        else
            {
            string json = request.downloadHandler.text;
            TransferResponse res = JsonUtility.FromJson<TransferResponse>(json);

            string resultMsg = $"✅ Status: {res.status}\n🔗 Hash: {res.hash}\n⛽ Gas Used: {res.gas_used}";
            Debug.Log(resultMsg);

            if (indexValue == 1)
                {
                if (resultText1 != null)
                    //resultText1.text = resultMsg;
                    resultText1.text = "Completed";
                GamePlayManager.instance.OnShowAds();
                noThanks.interactable = true;
                backButton.interactable = true;
                restartBtn.interactable = true;
                }
            else
                {
                if (resultText2 != null)
                    //resultText2.text = resultMsg;
                    resultText2.text = "Completed";
                GamePlayManager.instance.CommitToAptos();
                noThanks.interactable = true;
                }
            }
        //noThanks.interactable = true;
        //backButton.interactable = true;
        //restartBtn.interactable = true;
        }

    [System.Serializable]
    public class TransferRequestData
        {
        public string mnemonic;
        public string to;
        public string amount; // JSON expects string for large numbers
        }

    [System.Serializable]
    public class TransferResponse
        {
        public string hash;
        public string status;
        public int gas_used;
        }
    }
