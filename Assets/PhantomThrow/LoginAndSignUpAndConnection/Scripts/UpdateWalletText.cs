using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class UpdateWalletText : MonoBehaviour
{

    public TextMeshProUGUI walletAddressValue;
    public TextMeshProUGUI usernameValue;
    public TextMeshProUGUI mnemonicsValue;

    public string walletAddress; // Gán ví APT muốn xem
    public TMP_Text balanceText; // Gán object "BalanceValue" trong Inspector
    //private string apiBaseUrl = "http://localhost:3001";
    private string apiBaseUrl = APIEndpoints.BaseWalletURL;
    // Start is called before the first frame update
    void Start()
    {
        usernameValue.text = UserSession.Instance.Username;
        walletAddressValue.text = UserSession.Instance.WalletAddress;
        mnemonicsValue.text = UserSession.Instance.Mnemonics;

        walletAddress = UserSession.Instance.WalletAddress;

        if (!string.IsNullOrEmpty(walletAddress))
            {
            StartCoroutine(FetchBalance(walletAddress));
            }
        else
            {
            Debug.LogError("❌ Wallet address is not set!");
            }

        }

    IEnumerator FetchBalance(string address)
        {
        string url = $"{apiBaseUrl}/wallet/balance/{address}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            {
            Debug.LogError("❌ Failed to fetch balance: " + request.error);
            balanceText.text = "Balance: Error";
            }
        else
            {
            string json = request.downloadHandler.text;
            BalanceResponse data = JsonUtility.FromJson<BalanceResponse>(json);

            decimal rawBalance = decimal.Parse(data.balance); // hoặc double.Parse
            decimal aptBalance = rawBalance / 100_000_000m;

            balanceText.text = $"Balance: {aptBalance} APT";
            }
        }

    [System.Serializable]
    public class BalanceResponse
        {
        public string address;
        public string balance;
        }
    }
