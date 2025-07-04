using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MainMenu();
        }
    }
    public void ChangeScene(string sceneName)
    {
        if (GameManager.Instance != null) { GameManager.Instance.levelName = sceneName; }
        SceneManager.LoadScene(sceneName);
    }

    public void LogOut()
    {
        SaveUserId.Instance.UserId = string.Empty;
        SceneManager.LoadScene("LandingPage");
    }
}