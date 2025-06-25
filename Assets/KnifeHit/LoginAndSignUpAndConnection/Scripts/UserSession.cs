using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    public string Username { get; private set; }
    public string WalletAddress { get; private set; }
    public string Mnemonics { get; private set; }
    public int Score { get; private set; }

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Giữ qua scene
    }

    /// <summary>
    /// Lưu thông tin người dùng sau khi login hoặc lấy dữ liệu từ server
    /// </summary>
    public void SetUserData(string username, string walletAddress, string mnemonics, int score)
    {
        Username = username;
        WalletAddress = walletAddress;
        Mnemonics = mnemonics;
        Score = score;
    }

    /// <summary>
    /// Reset toàn bộ dữ liệu người dùng
    /// </summary>
    public void ClearUserData()
    {
        Username = null;
        WalletAddress = null;
        Mnemonics = null;
        Score = 0;
    }
}
