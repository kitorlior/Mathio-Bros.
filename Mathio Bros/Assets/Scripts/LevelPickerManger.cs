using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPickerManger : MonoBehaviour
{
    public void Level1clicked()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Level2clicked()
    {
        SceneManager.LoadScene("Level2");
    }
    public void Level3clicked()
    {
        SceneManager.LoadScene("Level3");
    }

    public void OnSignOutClicked()
    {
        SceneManager.LoadScene("LandingPage");
    }
    public void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
