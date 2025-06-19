using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class FirebaseAPIManager : MonoBehaviour
{
    private const string SERVER_URL = "http://localhost:3000"; // Replace with real IP/host
    private static FirebaseAPIManager _instance;
    public static FirebaseAPIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new("FirebaseAPIManager");
                _instance = go.AddComponent<FirebaseAPIManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
        public string name;
        public string email;
        public string level;
        public int score;
        public float timeSpent;
        public string classId;
        public string lastActive;
    }

    [System.Serializable]
    public class AuthData
    {
        public string email;
        public string password;
        public string username; // For signup only
    }

    // Callback delegates
    public delegate void AuthCallback(bool success, string message, string playerId);
    public delegate void DataCallback(bool success, string jsonData);

    // 1. Authentication Methods
    public IEnumerator SignUp(string email, string password, string username, AuthCallback callback)
    {
        AuthData authData = new()
        {
            email = email,
            password = password,
            username = username
        };

        string json = JsonUtility.ToJson(authData);

        using UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/signup", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            callback(false, request.error, null);
        }
        else
        {
            var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            callback(response.success, response.message, response.uid);
        }
    }

    public IEnumerator Login(string email, string password, AuthCallback callback)
    {
        AuthData authData = new AuthData
        {
            email = email,
            password = password
        };

        string json = JsonUtility.ToJson(authData);

        using (UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                callback(false, request.error, null);
            }
            else
            {
                var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                callback(response.success, response.message, response.uid);
            }
        }
    }

    // 2. Player Data Methods (existing)
    public IEnumerator SavePlayerData(string playerId, string levelName, int score, float timeSpent, DataCallback callback = null)
    {
        PlayerData data = new PlayerData
        {
            playerId = playerId,
            level = levelName,
            score = score,
            timeSpent = timeSpent
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/savePlayerData", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;
            callback?.Invoke(success, success ? "Saved!" : request.error);
        }
    }

    public IEnumerator GetPlayerData(string playerId, DataCallback callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{SERVER_URL}/getPlayerData/{playerId}"))
        {
            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;
            callback?.Invoke(success, success ? request.downloadHandler.text : request.error);
        }
    }

    // Helper class for JSON responses
    [System.Serializable]
    private class AuthResponse
    {
        public bool success;
        public string message;
        public string uid;
    }

    public IEnumerator UpdatePlayerData(string playerId, string levelName, int score, float timeSpent, DataCallback callback = null)
    {
        var updatePayload = new PlayerData
        {
            playerId = playerId,
            level = levelName,
            score = score,
            timeSpent = timeSpent
        };

        string json = JsonUtility.ToJson(updatePayload);

        using (UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/updatePlayerData", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;
            callback?.Invoke(success, success ? request.downloadHandler.text : request.error);
        }
    }
}