using UnityEngine;
using UnityEngine.SceneManagement; // 👈 cần để chuyển scene

public class SceneManagerScript : MonoBehaviour
    {
    // Gọi hàm này để chuyển tới LeaderboardScene
    public void GoToLeaderboardScene()
        {
        SceneManager.LoadScene("LeaderboardScene");
        }

    // Gọi hàm này để chuyển tới HomeScene
    public void GoToHomeScene()
        {
        SceneManager.LoadScene("HomeScene");
        }
    }
