using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

[System.Serializable]
public class TxUser
    {
    public string username;
    public string walletaddress;
    public int tx_count;
    }

[System.Serializable]
public class TxLeaderboardWrapper
    {
    public TxUser[] users;
    public string message;
    }

public class LeaderboardTxCount : MonoBehaviour
    {
    public GameObject[] entries; // Gán GameObject 01 → 05 trong Inspector
    private string apiUrl = "http://localhost:3000/top-tx-count"; // Đổi nếu server đã deploy

    void Start()
        {
        StartCoroutine(FetchLeaderboard());
        }

    IEnumerator FetchLeaderboard()
        {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            {
            Debug.LogError("❌ Error fetching tx_count leaderboard: " + request.error);
            }
        else
            {
            string rawJson = request.downloadHandler.text;
            string fixedJson = ExtractUsersJson(rawJson);

            TxLeaderboardWrapper leaderboard = JsonUtility.FromJson<TxLeaderboardWrapper>(fixedJson);

            if (leaderboard == null || leaderboard.users == null)
                {
                Debug.LogError("❌ Failed to parse tx_count users from leaderboard JSON.");
                }
            else
                {
                Debug.Log($"✅ Loaded {leaderboard.users.Length} tx users from server.");
                PopulateUI(leaderboard.users);
                }
            }
        }

    void PopulateUI(TxUser[] users)
        {
        Debug.Log("📦 users is null? " + (users == null));
        Debug.Log("📦 users.Length: " + (users != null ? users.Length.ToString() : "N/A"));

        for (int i = 0; i < entries.Length; i++)
            {
            if (users == null)
                {
                Debug.LogError("❌ users array is null! Cannot populate UI.");
                break;
                }

            if (i >= users.Length)
                {
                Debug.Log($"ℹ️ i = {i}, but users.Length = {users.Length} → breaking.");
                break;
                }

            Transform entry = entries[i].transform;
            Debug.Log($"🔍 Populating tx_count entry {i + 1}: {entry.name}");

            var noObj = entry.Find("No");
            var userObj = entry.Find("Username");
            var walletObj = entry.Find("WalletAddress");
            var txObj = entry.Find("TxCount"); // 👈 cần GameObject con tên "TxCount"

            if (noObj == null) Debug.LogError($"❌ entry[{i}]: 'No' not found.");
            if (userObj == null) Debug.LogError($"❌ entry[{i}]: 'Username' not found.");
            if (walletObj == null) Debug.LogError($"❌ entry[{i}]: 'WalletAddress' not found.");
            if (txObj == null) Debug.LogError($"❌ entry[{i}]: 'TxCount' not found.");

            if (noObj != null) noObj.GetComponent<TMP_Text>().text = (i + 1).ToString();
            if (userObj != null) userObj.GetComponent<TMP_Text>().text = users[i].username;
            if (walletObj != null) walletObj.GetComponent<TMP_Text>().text = users[i].walletaddress;
            if (txObj != null) txObj.GetComponent<TMP_Text>().text = users[i].tx_count.ToString();

            Debug.Log($"✅ Done tx_count entry {i + 1}: {users[i].username}");
            }
        }

    // Trích riêng phần users từ JSON
    string ExtractUsersJson(string rawJson)
        {
        int usersIndex = rawJson.IndexOf("\"users\":");
        if (usersIndex == -1)
            {
            Debug.LogError("❌ Cannot find 'users' in JSON.");
            return "{}";
            }

        string usersJsonPart = rawJson.Substring(usersIndex);
        return "{" + usersJsonPart.TrimEnd('}', '\n', '\r') + "}";
        }
    }
