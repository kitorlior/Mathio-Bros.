using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProfilePage : MonoBehaviour
{
    public Text nameText;
    public Text hoursPlayedText;
    public Text levelsSolvedText;
    public Text averageTimeText;
    public Button backButton;

    private void Start()
    {
        // Load player data (example values)
        nameText.text = "Player Name: " + PlayerPrefs.GetString("PlayerName", "Guest");
        hoursPlayedText.text = "Hours Played: " + PlayerPrefs.GetFloat("HoursPlayed", 0).ToString("F2");
        levelsSolvedText.text = "Levels Solved: " + PlayerPrefs.GetInt("LevelsSolved", 0);
        averageTimeText.text = "Average Solving Time: " + PlayerPrefs.GetFloat("AverageTime", 0).ToString("F2") + "s";

        backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}