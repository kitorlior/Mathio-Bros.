using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startGameButton;
    public Button chooseLevelButton;
    public Button profileButton;
    public Button tutorialButton;
    public Button signOutButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        chooseLevelButton.onClick.AddListener(OnChooseLevelClicked);
        profileButton.onClick.AddListener(OnProfileClicked);
        tutorialButton.onClick.AddListener(OnTutorialClicked);
        signOutButton.onClick.AddListener(OnSignOutClicked);
    }

    private void OnStartGameClicked()
    {
        // Check if this is the first time playing
        int lastLevel = PlayerPrefs.GetInt("LastLevel", 1); // Default to Level 1
        SceneManager.LoadScene($"Level{lastLevel}");
    }

    private void OnChooseLevelClicked()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    private void OnProfileClicked()
    {
        SceneManager.LoadScene("Profile");
    }

    private void OnTutorialClicked()
    {
        SceneManager.LoadScene("Tutorial");
    }
    private void OnSignOutClicked()
    {
        SceneManager.LoadScene("LandingPage");
    }
}