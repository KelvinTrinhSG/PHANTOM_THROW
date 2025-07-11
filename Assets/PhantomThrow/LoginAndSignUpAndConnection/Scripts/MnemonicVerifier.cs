using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MnemonicVerifier : MonoBehaviour
    {
    public TMP_InputField mnemonicsInput;
    public TMP_Text feedbackText;

    //private string backendUrl = "http://localhost:3000/get-user-by-mnemonic";
    private string backendUrl = APIEndpoints.GetUserByMnemonic;


    [ContextMenu("Verify Mnemonic")]
    public void OnClickVerifyMnemonic()
        {
        string mnemonic = mnemonicsInput.text.Trim();

        if (string.IsNullOrEmpty(mnemonic))
            {
            feedbackText.text = "❌ Mnemonics cannot be empty.";
            return;
            }

        StartCoroutine(CheckMnemonicInBackend(mnemonic));
        }

    IEnumerator CheckMnemonicInBackend(string mnemonic)
        {
        // Tạo JSON body
        string jsonBody = JsonUtility.ToJson(new MnemonicRequest { mnemonic = mnemonic });

        UnityWebRequest request = new UnityWebRequest(backendUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
            {
            Debug.Log("✅ Response: " + request.downloadHandler.text);
            UserWalletResponseWrapper wrapper = JsonUtility.FromJson<UserWalletResponseWrapper>("{\"data\":" + request.downloadHandler.text + "}");

            // Lưu vào Singleton
            UserSession.Instance.SetUserData(
                wrapper.data.username,
                wrapper.data.walletaddress,
                mnemonic,
                0 // Score chưa có
            );

            SceneManager.LoadScene("WalletInform");
            }
        else
            {
            Debug.LogWarning("❌ User not found. Must signup first.");
            feedbackText.text = "❌ Please signup first";
            }
        }

    [System.Serializable]
    class MnemonicRequest
        {
        public string mnemonic;
        }

    [System.Serializable]
    class UserWalletResponse
        {
        public string username;
        public string walletaddress;
        }

    // Wrapper để JsonUtility hiểu được response không có root
    [System.Serializable]
    class UserWalletResponseWrapper
        {
        public UserWalletResponse data;
        }
    }
