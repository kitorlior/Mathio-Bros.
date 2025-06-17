using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void logOut()
    {
        SaveUserId.Instance.UserId = string.Empty;
        SceneManager.LoadScene("LandingPage");
    }
}