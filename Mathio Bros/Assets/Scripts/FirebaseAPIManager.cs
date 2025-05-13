using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FirebaseAPIManager : MonoBehaviour
{
    private const string SERVER_URL = "http://localhost:3000"; // Replace with deployed URL

    // Save player data to server
    public IEnumerator SavePlayerData(string playerId, int level, int score, float timeSpent)
    {
        string jsonData = JsonUtility.ToJson(new PlayerData(playerId, level, score, timeSpent));
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm($"{SERVER_URL}/savePlayerData", jsonData))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to save data: " + request.error);
            }
            else
            {
                Debug.Log("Data saved successfully!");
            }
        }
    }

    // Fetch player data from server
    public IEnumerator GetPlayerData(string playerId)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{SERVER_URL}/getPlayerData/{playerId}"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch data: " + request.error);
            }
            else
            {
                PlayerData data = JsonUtility.FromJson<PlayerData>(request.downloadHandler.text);
                Debug.Log($"Level: {data.level}, Score: {data.score}");
            }
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerId;
    public int level;
    public int score;
    public float timeSpent;

    public PlayerData(string playerId, int level, int score, float timeSpent)
    {
        this.playerId = playerId;
        this.level = level;
        this.score = score;
        this.timeSpent = timeSpent;
    }
}