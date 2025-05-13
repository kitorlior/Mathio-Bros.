using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    private const string SERVER_URL = "http://localhost:3000";

    // Sign Up
    public IEnumerator SignUp(string email, string password, string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("username", username);

        using (UnityWebRequest request = UnityWebRequest.Post($"{SERVER_URL}/signup", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Signup failed: " + request.error);
            }
            else
            {
                Debug.Log("Signup successful!");
                // Parse response for UID if needed
            }
        }
    }

    // Login
    public IEnumerator Login(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest request = UnityWebRequest.Post($"{SERVER_URL}/login", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Login failed: " + request.error);
            }
            else
            {
                Debug.Log("Login successful!");
                // Parse user data from response
            }
        }
    }

    // Check if username/email exists
    public IEnumerator CheckUserExists(string field, string value)
    {
        string url = $"{SERVER_URL}/checkUser?field={field}&value={value}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Check failed: " + request.error);
            }
            else
            {
                bool exists = JsonUtility.FromJson<UserExistsResponse>(request.downloadHandler.text).exists;
                Debug.Log(exists ? "User exists!" : "User does not exist.");
            }
        }
    }
}

[System.Serializable]
public class UserExistsResponse
{
    public bool exists;
}