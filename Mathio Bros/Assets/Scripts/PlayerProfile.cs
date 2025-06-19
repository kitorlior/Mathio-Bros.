using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static FirebaseAPIManager;
using UnityEngine.Networking;

public class ProfilePage : MonoBehaviour
{
    public Text hoursPlayedText;
    public Text LevelText;
    public Text scoreText;

    private void Start()
    {

        StartCoroutine(FirebaseAPIManager.Instance.GetPlayerData(SaveUserId.Instance.UserId, (success, dataOrError) =>
        {
            if (success)
            {
                PlayerData data = JsonUtility.FromJson<PlayerData>(dataOrError);

                // Update UI
                hoursPlayedText.text = $"Minutes Played: {(data.timeSpent / 60):F2}";
                LevelText.text = $"Last Level Solved: {data.level}";
                scoreText.text = $"Score: {data.score}";
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