using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class PlayerSession : MonoBehaviour
    {
    public static PlayerSession Instance { get; private set; }

    public string Username { get; set; }

    public int AppleUnwritten { get; set; }
    public int AppleWritten { get; set; }
    public int SwordCollected { get; set; }
    public int TxCount { get; set; }

    //private string getAppleUnwrittenUrl = "http://localhost:3000/get-apple-unwritten";
    private string getAppleUnwrittenUrl = APIEndpoints.GetAppleUnwritten;

    private void Awake()
        {
        if (Instance == null)
            {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại qua các scene
            }
        else
            {
            Destroy(gameObject); // Chỉ giữ 1 instance
            }
        }

    private void Start()
        {

        Username = UserSession.Instance.Username;


        //Bất kỳ khi nào muốn update singleton và text cho phần hiển thị SQL data thì gọi các hàm này
        if (!string.IsNullOrEmpty(Username))
            {
            StartCoroutine(GetAppleUnwrittenFromServer(Username));
            StartCoroutine(FetchAppleWritten(Username));
            StartCoroutine(FetchSwordCollected(Username));
            StartCoroutine(FetchTxCount(Username));
            }
        }

    public void UpdateAllUI() {
        StartCoroutine(GetAppleUnwrittenFromServer(Username));
        StartCoroutine(FetchAppleWritten(Username));
        StartCoroutine(FetchSwordCollected(Username));
        StartCoroutine(FetchTxCount(Username));
        }

    public void FetchAppleUnwritten()
        {
        if (!string.IsNullOrEmpty(Username))
            {
            StartCoroutine(GetAppleUnwrittenFromServer(Username));
            }
        else
            {
            Debug.LogError("❌ Username is not set!");
            }
        }

    private IEnumerator GetAppleUnwrittenFromServer(string username)
        {
        string url = $"{getAppleUnwrittenUrl}?username={UnityWebRequest.EscapeURL(username)}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
                {
                Debug.LogError("❌ Error fetching apple_unwritten: " + www.error);
                }
            else
                {
                try
                    {
                    AppleUnwrittenResponse response = JsonUtility.FromJson<AppleUnwrittenResponse>(www.downloadHandler.text);
                    AppleUnwritten = response.apple_unwritten;
                    Debug.Log($"✅ AppleUnwritten set to: {AppleUnwritten}");
                    GeneralFunction.intance.appleLbl.text = AppleUnwritten.ToString();
                    }
                catch (Exception ex)
                    {
                    Debug.LogError("❌ Failed to parse response: " + ex.Message);
                    GeneralFunction.intance.appleLbl.text = "0";
                    }
                }
            }
        }

    private IEnumerator FetchAppleWritten(string username)
        {
        //string url = $"http://localhost:3000/get-apple-written?username={username}";
        string url = $"{APIEndpoints.GetAppleWrittenBase}?username={username}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
                {
                Debug.LogError("❌ Error fetching apple_written: " + request.error);
                GeneralFunction.intance.appleLblOnchain.text = "0";
                }
            else
                {
                AppleWrittenResponse response = JsonUtility.FromJson<AppleWrittenResponse>(request.downloadHandler.text);
                AppleWritten = response.apple_written;
                GeneralFunction.intance.appleLblOnchain.text = AppleWritten.ToString();
                Debug.Log($"✅ AppleWritten fetched for {username}: {AppleWritten}");
                }
            }
        }

    private IEnumerator FetchSwordCollected(string username)
        {
        //string url = $"http://localhost:3000/get-sword-collected?username={username}";
        string url = $"{APIEndpoints.GetSwordCollectedBase}?username={username}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
                {
                Debug.LogError("❌ Error fetching sword_collected: " + request.error);
                GeneralFunction.intance.swordOnchain.text = "0";
                }
            else
                {
                SwordCollectedResponse response = JsonUtility.FromJson<SwordCollectedResponse>(request.downloadHandler.text);
                SwordCollected = response.sword_collected;
                GeneralFunction.intance.swordOnchain.text = SwordCollected.ToString();
                Debug.Log($"✅ SwordCollected fetched for {username}: {SwordCollected}");
                }
            }
        }

    private IEnumerator FetchTxCount(string username)
        {
        //string url = $"http://localhost:3000/get-tx-count?username={username}";
        string url = $"{APIEndpoints.GetTransactionCountBase}?username={username}";


        using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
                {
                Debug.LogError("❌ Error fetching tx_count: " + request.error);
                GeneralFunction.intance.txOnchain.text = "0";
                }
            else
                {
                TxCountResponse response = JsonUtility.FromJson<TxCountResponse>(request.downloadHandler.text);
                TxCount = response.tx_count;
                GeneralFunction.intance.txOnchain.text = TxCount.ToString();
                Debug.Log($"✅ TxCount fetched for {username}: {TxCount}");
                }
            }
        }

    [System.Serializable]
    private class TxCountResponse
        {
        public string username;
        public int tx_count;
        }

    [System.Serializable]
    private class SwordCollectedResponse
        {
        public string username;
        public int sword_collected;
        }

    [System.Serializable]
    private class AppleWrittenResponse
        {
        public string username;
        public int apple_written;
        }

    [Serializable]
    private class AppleUnwrittenResponse
        {
        public string username;
        public int apple_unwritten;
        }
    }
