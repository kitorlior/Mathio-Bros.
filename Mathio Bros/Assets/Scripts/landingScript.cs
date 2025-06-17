using UnityEngine;
using UnityEngine.SceneManagement;

public class LandingPage : MonoBehaviour
{
    public void ChangeScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}