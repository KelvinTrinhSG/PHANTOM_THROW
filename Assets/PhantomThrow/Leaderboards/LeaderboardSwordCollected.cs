using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

[System.Serializable]
public class SwordUser
    {
    public string username;
    public string walletaddress;
    public int sword_collected;
    }

[System.Serializable]
public class SwordLeaderboardWrapper
    {
    public SwordUser[] users;
    public string message;
    }

public class LeaderboardSwordCollected : MonoBehaviour
    {
    public GameObject[] entries; // Gán các GameObject 01 → 05 trong Inspector
    //private string apiUrl = "http://localhost:3000/top-sword-collected"; // Đổi nếu deploy server
    private string apiUrl = APIEndpoints.GetTopSwordCollected;

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
            Debug.LogError("❌ Error fetching leaderboard: " + request.error);
            }
        else
            {
            string rawJson = request.downloadHandler.text;
            string fixedJson = ExtractUsersJson(rawJson);

            SwordLeaderboardWrapper leaderboard = JsonUtility.FromJson<SwordLeaderboardWrapper>(fixedJson);

            if (leaderboard == null || leaderboard.users == null)
                {
                Debug.LogError("❌ Failed to parse users from leaderboard JSON.");
                }
            else
                {
                Debug.Log($"✅ Loaded {leaderboard.users.Length} sword users from server.");
                PopulateUI(leaderboard.users);
                }
            }
        }

    void PopulateUI(SwordUser[] users)
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
            Debug.Log($"🔍 Populating sword entry {i + 1}: {entry.name}");

            var noObj = entry.Find("No");
            var userObj = entry.Find("Username");
            var walletObj = entry.Find("WalletAddress");
            var swordObj = entry.Find("Sword"); // 👈 cần object con tên "Sword" trong mỗi entry

            if (noObj == null) Debug.LogError($"❌ entry[{i}]: 'No' not found.");
            if (userObj == null) Debug.LogError($"❌ entry[{i}]: 'Username' not found.");
            if (walletObj == null) Debug.LogError($"❌ entry[{i}]: 'WalletAddress' not found.");
            if (swordObj == null) Debug.LogError($"❌ entry[{i}]: 'Sword' not found.");

            if (noObj != null) noObj.GetComponent<TMP_Text>().text = (i + 1).ToString();
            if (userObj != null) userObj.GetComponent<TMP_Text>().text = users[i].username;
            if (walletObj != null) walletObj.GetComponent<TMP_Text>().text = users[i].walletaddress;
            if (swordObj != null) swordObj.GetComponent<TMP_Text>().text = users[i].sword_collected.ToString();

            Debug.Log($"✅ Done sword entry {i + 1}: {users[i].username}");
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
