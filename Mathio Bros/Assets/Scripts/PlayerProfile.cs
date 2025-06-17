using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProfilePage : MonoBehaviour
{
    public Text nameText;
    public Text hoursPlayedText;
    public Text levelsSolvedText;
    public Text averageTimeText;

    private void Start()
    {
        // Load player data (example values)
        nameText.text = "Player Name: " + PlayerPrefs.GetString("PlayerName", "Guest");
        hoursPlayedText.text = "Hours Played: " + PlayerPrefs.GetFloat("HoursPlayed", 0).ToString("F2");
        levelsSolvedText.text = "Levels Solved: " + PlayerPrefs.GetInt("LevelsSolved", 0);
        averageTimeText.text = "Average Solving Time: " + PlayerPrefs.GetFloat("AverageTime", 0).ToString("F2") + "s";

        StartCoroutine(FirebaseAPIManager.Instance.GetPlayerData(SaveUserId.Instance.UserId, (success, dataOrError) =>
        {
            if (success)
            {
                Debug.Log("Received player data: " + dataOrError);
                // Parse JSON: PlayerData data = JsonUtility.FromJson<PlayerData>(dataOrError);
            }
            else
            {
                Debug.LogError("Failed to fetch data: " + dataOrError);
            }
        }));
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}