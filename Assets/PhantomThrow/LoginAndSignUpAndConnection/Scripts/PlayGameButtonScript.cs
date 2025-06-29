using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameButton : MonoBehaviour
    {
    public void OnClickPlayGame()
        {
        Debug.Log("🎮 Loading HomeScene...");
        SceneManager.LoadScene("HomeScene");
        }
    }
