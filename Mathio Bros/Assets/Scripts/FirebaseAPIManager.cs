using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FirebaseAPIManager : MonoBehaviour
{
    public const string SERVER_URL = "localhost:3000"; // Replace with real IP/host

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

    public IEnumerator SavePlayerData(string playerId, int level, int score, float timeSpent)
    {
        PlayerData data = new PlayerData(playerId, level, score, timeSpent);
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/savePlayerData", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Save failed: {request.error}");
        }
        else
        {
            Debug.Log("Player data saved successfully!");
        }
    }

    public IEnumerator GetPlayerData(string playerId)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{SERVER_URL}/getPlayerData/{playerId}");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Get failed: {request.error}");
        }
        else
        {
            Debug.Log("Player data: " + request.downloadHandler.text);
        }
    }
}
