using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System;

[System.Serializable]
public class AppleUser
    {
    public string username;
    public string walletaddress;
    public int apple_written;
    }

[System.Serializable]
public class AppleLeaderboardWrapper
    {
    public AppleUser[] users;
    public string message;
    }

public class LeaderboardAppleWritten : MonoBehaviour
    {
    public GameObject[] entries; // Gán các GameObject 01 → 05 trong Inspector
    private string apiUrl = "http://localhost:3000/top-apple-written"; // Đổi nếu deploy server

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

            AppleLeaderboardWrapper leaderboard = JsonUtility.FromJson<AppleLeaderboardWrapper>(fixedJson);

            if (leaderboard == null || leaderboard.users == null)
                {
                Debug.LogError("❌ Failed to parse users from leaderboard JSON.");
                }
            else
                {
                Debug.Log($"✅ Loaded {leaderboard.users.Length} users from server.");
                PopulateUI(leaderboard.users);
                }
            }
        }

    void PopulateUI(AppleUser[] users)
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
            Debug.Log($"🔍 Populating entry {i + 1}: {entry.name}");

            var noObj = entry.Find("No");
            var userObj = entry.Find("Username");
            var walletObj = entry.Find("WalletAddress");
            var appleObj = entry.Find("Apple");

            if (noObj == null) Debug.LogError($"❌ entry[{i}]: 'No' not found.");
            if (userObj == null) Debug.LogError($"❌ entry[{i}]: 'Username' not found.");
            if (walletObj == null) Debug.LogError($"❌ entry[{i}]: 'WalletAddress' not found.");
            if (appleObj == null) Debug.LogError($"❌ entry[{i}]: 'Apple' not found.");

            if (noObj != null) noObj.GetComponent<TMP_Text>().text = (i + 1).ToString();
            if (userObj != null) userObj.GetComponent<TMP_Text>().text = users[i].username;
            if (walletObj != null) walletObj.GetComponent<TMP_Text>().text = users[i].walletaddress;
            if (appleObj != null) appleObj.GetComponent<TMP_Text>().text = users[i].apple_written.ToString();

            Debug.Log($"✅ Done entry {i + 1}: {users[i].username}");
            }
        }

    // Fix Json: Extract phần users từ JSON trả về
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
